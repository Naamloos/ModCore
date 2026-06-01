import DashboardLayout from "@/Layouts/DashboardLayout";
import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { PagePropsWith } from "@/Types/PageProps";
import { Button } from "@/Components/ui/button";
import { useState, useEffect } from "react";
import { Input } from "@/Components/ui/input";
import { useToast } from "@/hooks/use-toast";
import { router } from "@inertiajs/react";
import ModCoreInfraction from "@/Types/DatabaseTypes/ModCoreInfraction";
import ModCoreInfractionType from "@/Types/DatabaseTypes/ModCoreInfractionType";

import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/Components/ui/card";

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/Components/ui/dialog";

import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/Components/ui/table";

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/Components/ui/alert-dialog";

interface ConfigureInfractionsProps {
  server: DiscordGuild;
  infractions: ModCoreInfraction[];
}

export default function Configure({
  server,
  infractions: initialInfractions,
}: PagePropsWith<ConfigureInfractionsProps>) {
  let icon = server?.icon ? server.icon : null;
  if (icon) {
    const ext = icon.includes("a_") ? ".gif" : ".png";
    icon = "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
  } else {
    icon = "https://cdn.discordapp.com/embed/avatars/0.png";
  }

  const [infractions, setInfractions] =
    useState<ModCoreInfraction[]>(initialInfractions);
  const [page, setPage] = useState(1);
  const itemsPerPage = 10;
  const [search, setSearch] = useState("");
  const [filteredInfractions, setFilteredInfractions] =
    useState<ModCoreInfraction[]>(initialInfractions);
  const [infractionToDelete, setInfractionToDelete] =
    useState<ModCoreInfraction | null>(null);
  const { toast } = useToast();

  useEffect(() => {
    const results = initialInfractions.filter((infraction) => {
      return (
        infraction.user_id.toLowerCase().includes(search.toLowerCase()) ||
        infraction.responsible_moderator_id
          .toLowerCase()
          .includes(search.toLowerCase()) ||
        infraction.reason.toLowerCase().includes(search.toLowerCase())
      );
    });
    setFilteredInfractions(results);
    setPage(1); // Reset to first page when search changes
  }, [search, initialInfractions]);

  const pageCount = Math.ceil(filteredInfractions.length / itemsPerPage);
  const paginatedInfractions = filteredInfractions.slice(
    (page - 1) * itemsPerPage,
    page * itemsPerPage,
  );

  const getInfractionTypeName = (type: ModCoreInfractionType) => {
    return ModCoreInfractionType[type];
  };

  const handleDeleteInfraction = () => {
    if (infractionToDelete) {
      // Here you would implement the API call to delete the infraction
      // For now, just update the local state
      setInfractions(
        infractions.filter(
          (i) =>
            !(
              i.guild_id === infractionToDelete.guild_id &&
              i.user_id === infractionToDelete.user_id &&
              i.responsible_moderator_id ===
                infractionToDelete.responsible_moderator_id
            ),
        ),
      );
      setFilteredInfractions(
        filteredInfractions.filter(
          (i) =>
            !(
              i.guild_id === infractionToDelete.guild_id &&
              i.user_id === infractionToDelete.user_id &&
              i.responsible_moderator_id ===
                infractionToDelete.responsible_moderator_id
            ),
        ),
      );

      toast({
        title: "Infraction Deleted",
        description: `Infraction for user ${infractionToDelete.user_id} has been removed.`,
      });

      setInfractionToDelete(null);
    }
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
            {server.name}: Infraction Management
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
            <div className="flex justify-between items-center">
              <div>
                <CardTitle className="text-2xl">Infractions</CardTitle>
                <CardDescription>
                  Manage user infractions for this server.
                </CardDescription>
              </div>
              <Input
                type="search"
                placeholder="Search infractions..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="max-w-md"
              />
            </div>
          </CardHeader>
          <CardContent>
            <Table>
              <TableCaption>
                A list of all infractions for this server.
              </TableCaption>
              <TableHeader>
                <TableRow>
                  <TableHead>User ID</TableHead>
                  <TableHead>Type</TableHead>
                  <TableHead>Moderator</TableHead>
                  <TableHead>Reason</TableHead>
                  <TableHead>Notified</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {paginatedInfractions.map((infraction, index) => (
                  <TableRow key={index}>
                    <TableCell className="font-medium">
                      {infraction.user_id}
                    </TableCell>
                    <TableCell>
                      {getInfractionTypeName(infraction.infraction_type)}
                    </TableCell>
                    <TableCell>{infraction.responsible_moderator_id}</TableCell>
                    <TableCell>
                      {infraction.reason.length > 50
                        ? infraction.reason.substring(0, 50) + "..."
                        : infraction.reason}
                    </TableCell>
                    <TableCell>
                      {infraction.user_was_notified ? "Yes" : "No"}
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <Dialog>
                          <DialogTrigger asChild>
                            <Button variant="outline" size="sm">
                              View
                            </Button>
                          </DialogTrigger>
                          <DialogContent className="sm:max-w-[600px]">
                            <DialogHeader>
                              <DialogTitle>Infraction Details</DialogTitle>
                              <DialogDescription>
                                Information about the infraction for user ID:{" "}
                                {infraction.user_id}
                              </DialogDescription>
                            </DialogHeader>
                            <div className="grid gap-4 py-4">
                              <div className="grid grid-cols-4 items-center gap-4">
                                <span className="font-medium">Type:</span>
                                <span className="col-span-3">
                                  {getInfractionTypeName(
                                    infraction.infraction_type,
                                  )}
                                </span>
                              </div>
                              <div className="grid grid-cols-4 items-center gap-4">
                                <span className="font-medium">Moderator:</span>
                                <span className="col-span-3">
                                  {infraction.responsible_moderator_id}
                                </span>
                              </div>
                              <div className="grid grid-cols-4 items-center gap-4">
                                <span className="font-medium">
                                  User Notified:
                                </span>
                                <span className="col-span-3">
                                  {infraction.user_was_notified ? "Yes" : "No"}
                                </span>
                              </div>
                              <div className="grid grid-cols-4 gap-4">
                                <span className="font-medium">Reason:</span>
                                <div className="col-span-3 p-4 bg-gray-800 rounded-md whitespace-pre-wrap">
                                  {infraction.reason}
                                </div>
                              </div>
                            </div>
                          </DialogContent>
                        </Dialog>

                        <AlertDialog>
                          <AlertDialogTrigger asChild>
                            <Button
                              variant="destructive"
                              size="sm"
                              onClick={() => setInfractionToDelete(infraction)}
                            >
                              Delete
                            </Button>
                          </AlertDialogTrigger>
                          <AlertDialogContent>
                            <AlertDialogHeader>
                              <AlertDialogTitle>
                                Delete Infraction?
                              </AlertDialogTitle>
                              <AlertDialogDescription>
                                Are you sure you want to delete this infraction?
                                This action cannot be undone.
                              </AlertDialogDescription>
                            </AlertDialogHeader>
                            <AlertDialogFooter>
                              <AlertDialogCancel
                                onClick={() => setInfractionToDelete(null)}
                              >
                                Cancel
                              </AlertDialogCancel>
                              <AlertDialogAction
                                onClick={handleDeleteInfraction}
                              >
                                Delete
                              </AlertDialogAction>
                            </AlertDialogFooter>
                          </AlertDialogContent>
                        </AlertDialog>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
                {paginatedInfractions.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={6} className="text-center py-8">
                      No infractions found.
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
            {filteredInfractions.length > 0 && (
              <div className="flex justify-between items-center mt-4">
                <Button
                  onClick={() => setPage(1)}
                  disabled={page === 1}
                  variant="outline"
                >
                  First
                </Button>
                <Button
                  onClick={() => setPage(page - 1)}
                  disabled={page === 1}
                  variant="outline"
                >
                  Previous
                </Button>
                <span>
                  Page {page} of {pageCount || 1}
                </span>
                <Button
                  onClick={() => setPage(page + 1)}
                  disabled={page === pageCount || pageCount === 0}
                  variant="outline"
                >
                  Next
                </Button>
                <Button
                  onClick={() => setPage(pageCount)}
                  disabled={page === pageCount || pageCount === 0}
                  variant="outline"
                >
                  Last
                </Button>
              </div>
            )}
          </CardContent>
        </Card>
      </DashboardLayout>
    </>
  );
}
