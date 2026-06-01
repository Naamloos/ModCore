import DashboardLayout from "@/Layouts/DashboardLayout";
import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { PagePropsWith } from "@/Types/PageProps";
import { Button } from "@/Components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/Components/ui/card";
import { useState } from "react";
import ModCoreRoleMenu from "../../../../Types/DatabaseTypes/ModCoreRoleMenu";
import { router, useForm } from "@inertiajs/react";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/Components/ui/table";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/Components/ui/dialog";
import { Label } from "@/Components/ui/label";
import { Input } from "@/Components/ui/input";
import DiscordChannelPicker from "@/Components/Forms/DiscordChannelPicker";
import DiscordRolePicker from "@/Components/Forms/DiscordRolePicker";
import { Separator } from "@/Components/ui/separator";
import { ScrollArea } from "@/Components/ui/scroll-area";
import { PlusCircle, Pencil, Trash2, ExternalLink, X } from "lucide-react";
import { DiscordRole } from "@/Types/DiscordTypes/DiscordRole";

interface ConfigureRoleMenusProps {
  server: DiscordGuild;
  menus: ModCoreRoleMenu[];
  roles: DiscordRole[];
}

interface RoleSelection {
  id: string;
  name: string;
  emoji?: string;
  description?: string;
}

export default function Configure({
  server,
  menus,
  roles,
}: PagePropsWith<ConfigureRoleMenusProps>) {
  let icon = server?.icon ? server.icon : null;
  if (icon) {
    const ext = icon.includes("a_") ? ".gif" : ".png";
    icon = "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
  } else {
    icon = "https://cdn.discordapp.com/embed/avatars/0.png";
  }

  // State to manage menus from the server
  const [roleMenus, setRoleMenus] = useState(menus || []);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [currentMenuId, setCurrentMenuId] = useState<number | null>(null);
  const [selectedRoles, setSelectedRoles] = useState<RoleSelection[]>([]);
  const [currentRoleId, setCurrentRoleId] = useState<string>("");

  // Form state for creating/editing menus
  const { data, setData, processing, errors, reset } = useForm({
    name: "",
    description: "",
    channelId: "",
    messageId: "",
    roles: [] as RoleSelection[],
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    // Update the form data to include the selected roles
    const formData = {
      ...data,
      roles: selectedRoles,
    };

    // Handle form submission by posting data to server
    console.log("Submitting form data:", formData);

    // Example API call (replace with your actual implementation)
    // post(route('rolemenu.store', server.id), formData, {
    //   onSuccess: () => {
    //     reset();
    //     setSelectedRoles([]);
    //     setIsDialogOpen(false);
    //   }
    // });

    // For demo, just close dialog
    reset();
    setSelectedRoles([]);
    setIsDialogOpen(false);
  };

  const handleEditMenu = (menu: any) => {
    setIsEditing(true);
    setCurrentMenuId(menu.id);
    setData({
      name: menu.name,
      description: menu.description || "",
      channelId: menu.channelId || "",
      messageId: menu.messageId || "",
      roles: [],
    });
    // If menu has roles, set them
    if (menu.roles) {
      setSelectedRoles(menu.roles);
    } else {
      setSelectedRoles([]);
    }
    setIsDialogOpen(true);
  };

  const handleDeleteMenu = (menuId: number) => {
    // Implement deletion logic here
    console.log("Deleting menu with ID:", menuId);

    // For demo, just filter the menu out
    setRoleMenus(roleMenus.filter((menu) => menu.id !== menuId));
  };

  const handleViewInDiscord = (menu: any) => {
    // Implement logic to open menu in Discord
    console.log("Viewing menu in Discord:", menu);

    // This would typically open a Discord URL
    if (menu.channelId && menu.messageId) {
      window.open(
        `https://discord.com/channels/${server.id}/${menu.channelId}/${menu.messageId}`,
        "_blank",
      );
    }
  };

  const openNewMenuDialog = () => {
    setIsEditing(false);
    reset();
    setSelectedRoles([]);
    setCurrentRoleId("");
    setIsDialogOpen(true);
  };

  const addRole = () => {
    if (!currentRoleId) return;

    const role = roles.find((r) => r.id === currentRoleId);
    if (!role) return;

    // Check if role is already selected
    if (selectedRoles.some((r) => r.id === currentRoleId)) return;

    const newRole: RoleSelection = {
      id: role.id,
      name: role.name,
      emoji: "", // Set default emoji or leave empty
      description: "", // Set default description or leave empty
    };

    setSelectedRoles([...selectedRoles, newRole]);
    setCurrentRoleId("");
  };

  const removeRole = (roleId: string) => {
    setSelectedRoles(selectedRoles.filter((role) => role.id !== roleId));
  };

  const updateRoleDetails = (
    roleId: string,
    field: keyof RoleSelection,
    value: string,
  ) => {
    setSelectedRoles(
      selectedRoles.map((role) =>
        role.id === roleId ? { ...role, [field]: value } : role,
      ),
    );
  };

  return (
    <>
      <DashboardLayout>
        <div className="md:flex items-center mb-4">
          <img
            src={icon}
            className="inline-block h-16 w-16 rounded-full mr-4"
            alt="User Avatar"
          />
          <h1 className="text-3xl font-extrabold tracking-tight text-white py-2">
            {server.name}: Role Menu Configuration
          </h1>
          <Button
            variant="outline"
            onClick={() => router.visit(`/dashboard/servers/${server.id}`)}
            className="ml-auto self-center"
          >
            Back to Overview
          </Button>
        </div>

        <Card className="w-full bg-gray-900 p-6 rounded-lg shadow-lg">
          <CardHeader>
            <CardTitle className="text-2xl">Role Menu Configuration</CardTitle>
            <CardDescription>
              Create and manage role menus for your server. Role menus allow
              users to assign themselves roles by clicking on reactions or
              buttons.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-6">
              <div className="flex justify-end mb-4">
                <Button
                  onClick={openNewMenuDialog}
                  className="flex items-center gap-2"
                >
                  <PlusCircle size={16} />
                  Create New Role Menu
                </Button>
              </div>

              {roleMenus.length > 0 ? (
                <div className="rounded-md border">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Name</TableHead>
                        <TableHead>Description</TableHead>
                        <TableHead>Channel</TableHead>
                        <TableHead>Roles</TableHead>
                        <TableHead className="text-right">Actions</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {roleMenus.map((menu) => (
                        <TableRow key={menu.id}>
                          <TableCell className="font-medium">
                            {menu.name}
                          </TableCell>
                          <TableCell>
                            {menu.description || "No description"}
                          </TableCell>
                          <TableCell>
                            {menu.channel_name || "Not set"}
                          </TableCell>
                          <TableCell>
                            {menu.roles ? menu.roles.length : 0} roles
                          </TableCell>
                          <TableCell className="text-right">
                            <div className="flex justify-end gap-2">
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => handleEditMenu(menu)}
                                title="Edit"
                              >
                                <Pencil size={16} />
                              </Button>
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => handleViewInDiscord(menu)}
                                title="View in Discord"
                              >
                                <ExternalLink size={16} />
                              </Button>
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => handleDeleteMenu(menu.id)}
                                title="Delete"
                                className="text-red-500 hover:text-red-600"
                              >
                                <Trash2 size={16} />
                              </Button>
                            </div>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              ) : (
                <div className="flex flex-col items-center justify-center p-8 text-center rounded-lg border border-dashed">
                  <h3 className="text-lg font-semibold mb-2">
                    No Role Menus Found
                  </h3>
                  <p className="text-sm text-gray-500 mb-4">
                    Create your first role menu to allow users to self-assign
                    roles in your server.
                  </p>
                  <Button onClick={openNewMenuDialog}>Create Role Menu</Button>
                </div>
              )}
            </div>
          </CardContent>
        </Card>
      </DashboardLayout>

      <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
        <DialogContent className="sm:max-w-[540px]">
          <form onSubmit={handleSubmit}>
            <DialogHeader>
              <DialogTitle>
                {isEditing ? "Edit Role Menu" : "Create New Role Menu"}
              </DialogTitle>
              <DialogDescription>
                {isEditing
                  ? "Update the details of your role menu."
                  : "Configure a new role menu for your server."}
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-4 py-4">
              <div className="space-y-2">
                <Label htmlFor="name">Menu Name</Label>
                <Input
                  id="name"
                  placeholder="Enter menu name"
                  value={data.name}
                  onChange={(e) => setData({ ...data, name: e.target.value })}
                />
                {errors.name && (
                  <p className="text-sm text-red-500">{errors.name}</p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="description">Description (Optional)</Label>
                <Input
                  id="description"
                  placeholder="Enter a description"
                  value={data.description}
                  onChange={(e) =>
                    setData({ ...data, description: e.target.value })
                  }
                />
              </div>

              <Separator />

              <div className="space-y-2">
                <Label htmlFor="channel">Discord Channel</Label>
                <DiscordChannelPicker
                  value={data.channelId}
                  onChange={(value) => setData({ ...data, channelId: value })}
                  server={server}
                />
                {errors.channelId && (
                  <p className="text-sm text-red-500">{errors.channelId}</p>
                )}
              </div>

              {isEditing && (
                <div className="space-y-2">
                  <Label htmlFor="messageId">Message ID (Optional)</Label>
                  <Input
                    id="messageId"
                    placeholder="Enter message ID"
                    value={data.messageId}
                    onChange={(e) =>
                      setData({ ...data, messageId: e.target.value })
                    }
                  />
                  <p className="text-xs text-gray-500">
                    Leave empty to create a new message
                  </p>
                </div>
              )}

              <Separator />

              <div className="space-y-4">
                <Label>Roles</Label>
                <div className="flex gap-2">
                  <div className="flex-1">
                    <DiscordRolePicker
                      label=""
                      placeholder="Select a role to add"
                      value={currentRoleId}
                      onUpdate={setCurrentRoleId}
                      required={false}
                    />
                  </div>
                  <Button
                    type="button"
                    onClick={addRole}
                    disabled={!currentRoleId}
                    className="mt-2"
                  >
                    Add Role
                  </Button>
                </div>

                {selectedRoles.length > 0 ? (
                  <div className="border rounded-md p-2">
                    <ScrollArea className="h-[200px]">
                      <div className="space-y-4">
                        {selectedRoles.map((role) => (
                          <div
                            key={role.id}
                            className="border rounded-md p-3 space-y-2"
                          >
                            <div className="flex justify-between items-center">
                              <div className="font-medium">{role.name}</div>
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => removeRole(role.id)}
                                className="h-8 w-8 p-0"
                              >
                                <X size={16} />
                              </Button>
                            </div>
                            <div className="grid grid-cols-1 gap-2">
                              <div>
                                <Label
                                  htmlFor={`emoji-${role.id}`}
                                  className="text-xs"
                                >
                                  Emoji (Optional)
                                </Label>
                                <Input
                                  id={`emoji-${role.id}`}
                                  placeholder="Emoji or emoji ID"
                                  value={role.emoji || ""}
                                  onChange={(e) =>
                                    updateRoleDetails(
                                      role.id,
                                      "emoji",
                                      e.target.value,
                                    )
                                  }
                                  className="mt-1"
                                />
                              </div>
                              <div>
                                <Label
                                  htmlFor={`description-${role.id}`}
                                  className="text-xs"
                                >
                                  Description (Optional)
                                </Label>
                                <Input
                                  id={`description-${role.id}`}
                                  placeholder="Role description"
                                  value={role.description || ""}
                                  onChange={(e) =>
                                    updateRoleDetails(
                                      role.id,
                                      "description",
                                      e.target.value,
                                    )
                                  }
                                  className="mt-1"
                                />
                              </div>
                            </div>
                          </div>
                        ))}
                      </div>
                    </ScrollArea>
                  </div>
                ) : (
                  <div className="text-center p-4 border border-dashed rounded-md">
                    <p className="text-sm text-gray-500">
                      No roles added yet. Add at least one role to create a
                      menu.
                    </p>
                  </div>
                )}
              </div>
            </div>

            <DialogFooter>
              <Button
                type="button"
                variant="outline"
                onClick={() => setIsDialogOpen(false)}
              >
                Cancel
              </Button>
              <Button
                type="submit"
                disabled={processing || selectedRoles.length === 0}
              >
                {isEditing ? "Save Changes" : "Create Menu"}
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>
    </>
  );
}
