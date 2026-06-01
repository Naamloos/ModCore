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
import { useState, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/Components/ui/dialog";
import { Input } from "@/Components/ui/input";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/Components/ui/table";
import { DiscordChannel } from "../../../../Types/DiscordTypes/DiscordChannel";
import { useToast } from "@/hooks/use-toast";
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
import { router } from "@inertiajs/react";

interface ConfigureTagsProps {
  server: DiscordGuild;
  tags: any[];
  channels: DiscordChannel[];
}

interface DatabaseTag {
  id: number;
  name: string;
  content: string;
  author_id: number;
  guild_id: string;
  createdAt: string;
  modifiedAt: string;
  channel_id: string | null;
  history: DatabaseTagHistory[];
}

interface DatabaseTagHistory {
  content: string;
  timestamp: string;
}

export default function Configure({
  user,
  server,
  channels,
  tags: initialTags,
}: PagePropsWith<ConfigureTagsProps>) {
  let icon = server?.icon ? server.icon : null;
  if (icon) {
    const ext = icon.includes("a_") ? ".gif" : ".png";
    icon = "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
  } else {
    icon = "https://cdn.discordapp.com/embed/avatars/0.png";
  }

  const [tags, setTags] = useState<DatabaseTag[]>(initialTags);
  const [page, setPage] = useState(1);
  const itemsPerPage = 5;
  const [search, setSearch] = useState("");
  const [filteredTags, setFilteredTags] = useState<DatabaseTag[]>(initialTags);
  //   const [, setOpen] = useState(false);
  //   const [newTagName, setNewTagName] = useState("");
  //   const [newTagContent, setNewTagContent] = useState("");
  const [selectedTagHistory, setSelectedTagHistory] =
    useState<DatabaseTag | null>(null);
  const [, setDeleteConfirmationOpen] = useState(false);
  const [tagToDelete, setTagToDelete] = useState<DatabaseTag | null>(null);
  const [, setTakeOwnershipConfirmationOpen] = useState(false);
  const [tagToTakeOwnership, setTagToTakeOwnership] =
    useState<DatabaseTag | null>(null);
  const { toast } = useToast();

  useEffect(() => {
    const results = initialTags.filter((tag) => {
      return (
        tag.name.toLowerCase().includes(search.toLowerCase()) ||
        tag.content.toLowerCase().includes(search.toLowerCase())
      );
    });
    setFilteredTags(results);
    setPage(1); // Reset to first page when search changes
  }, [search, initialTags]);

  const pageCount = Math.ceil(filteredTags.length / itemsPerPage);
  const paginatedTags = filteredTags.slice(
    (page - 1) * itemsPerPage,
    page * itemsPerPage,
  );

  //   const handleCreateTag = () => {
  //     // Implement your create tag logic here, e.g., API call
  //     const newTag: DatabaseTag = {
  //       id: Math.max(...tags.map((t) => t.id)) + 1, // generate a new ID
  //       name: newTagName,
  //       content: newTagContent,
  //       author_id: Number(user?.id ?? 0), // Assuming user has an ID
  //       guild_id: server.id,
  //       createdAt: new Date().toISOString(),
  //       modifiedAt: new Date().toISOString(),
  //       channel_id: null,
  //       history: [],
  //     };

  //     setTags([...tags, newTag]);
  //     setFilteredTags([...filteredTags, newTag]);
  //     setOpen(false);
  //     setNewTagName("");
  //     setNewTagContent("");
  //     toast({
  //       title: "Tag Created",
  //       description: `Tag "${newTag.name}" has been created.`,
  //     });
  //   };

  const handleDeleteTag = () => {
    if (tagToDelete) {
      // Implement your delete tag logic here, e.g., API call
      setTags(tags.filter((tag) => tag.id !== tagToDelete.id));
      setFilteredTags(filteredTags.filter((tag) => tag.id !== tagToDelete.id));
      setDeleteConfirmationOpen(false);
      setTagToDelete(null);
      toast({
        title: "Tag Deleted",
        description: `Tag "${tagToDelete.name}" has been deleted.`,
      });
    }
  };

  const handleTakeOwnership = () => {
    if (tagToTakeOwnership) {
      // Implement your take ownership logic here, e.g., API call
      const updatedTag = {
        ...tagToTakeOwnership,
        author_id: Number(user?.id ?? 0),
      };
      setTags(tags.map((tag) => (tag.id === updatedTag.id ? updatedTag : tag)));
      setFilteredTags(
        filteredTags.map((tag) =>
          tag.id === updatedTag.id ? updatedTag : tag,
        ),
      );
      setTakeOwnershipConfirmationOpen(false);
      setTagToTakeOwnership(null);
      toast({
        title: "Tag Ownership Taken",
        description: `You have taken ownership of tag "${updatedTag.name}".`,
      });
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
            {server.name}: Tag Management
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
                <CardTitle className="text-2xl">Tag Management</CardTitle>
                <CardDescription>Manage tags for this server.</CardDescription>
              </div>
              <Input
                type="search"
                placeholder="Search tags..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="max-w-md"
              />
            </div>
          </CardHeader>
          <CardContent>
            <Table>
              <TableCaption>A list of all tags for this server.</TableCaption>
              <TableHeader>
                <TableRow>
                  <TableHead className="w-[100px]">ID</TableHead>
                  <TableHead>Name</TableHead>
                  <TableHead>Channel</TableHead>
                  <TableHead>Content</TableHead>
                  <TableHead>History</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {paginatedTags.map((tag) => (
                  <TableRow key={tag.id}>
                    <TableCell className="font-medium">{tag.id}</TableCell>
                    <TableCell>{tag.name}</TableCell>
                    <TableCell>
                      #
                      {channels.find((x) => x.id == tag.channel_id)?.name ?? ""}
                    </TableCell>
                    <TableCell>{tag.content}</TableCell>
                    <TableCell>
                      <Dialog>
                        <DialogTrigger asChild>
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => setSelectedTagHistory(tag)}
                          >
                            View History
                          </Button>
                        </DialogTrigger>
                        <DialogContent className="sm:max-w-[425px]">
                          <DialogHeader>
                            <DialogTitle>Tag History</DialogTitle>
                            <DialogDescription>
                              History for tag: {tag.name}
                            </DialogDescription>
                          </DialogHeader>
                          {selectedTagHistory &&
                          selectedTagHistory.history &&
                          selectedTagHistory.history.length > 0 ? (
                            <Table>
                              <TableHeader>
                                <TableRow>
                                  <TableHead>Timestamp</TableHead>
                                  <TableHead>Content</TableHead>
                                </TableRow>
                              </TableHeader>
                              <TableBody>
                                {selectedTagHistory.history.map(
                                  (historyItem, index) => (
                                    <TableRow key={index}>
                                      <TableCell>
                                        {new Date(
                                          historyItem.timestamp,
                                        ).toLocaleString()}
                                      </TableCell>
                                      <TableCell>
                                        {historyItem.content}
                                      </TableCell>
                                    </TableRow>
                                  ),
                                )}
                              </TableBody>
                            </Table>
                          ) : (
                            <p>No history available for this tag.</p>
                          )}
                        </DialogContent>
                      </Dialog>
                    </TableCell>
                    <TableCell>
                      <div className="flex gap-2">
                        <AlertDialog>
                          <AlertDialogTrigger asChild>
                            <Button
                              variant="destructive"
                              size="sm"
                              onClick={() => setTagToDelete(tag)}
                            >
                              Delete
                            </Button>
                          </AlertDialogTrigger>
                          <AlertDialogContent>
                            <AlertDialogHeader>
                              <AlertDialogTitle>
                                Are you absolutely sure?
                              </AlertDialogTitle>
                              <AlertDialogDescription>
                                This action cannot be undone. Are you sure you
                                want to delete tag "{tag.name}"?
                              </AlertDialogDescription>
                            </AlertDialogHeader>
                            <AlertDialogFooter>
                              <AlertDialogCancel
                                onClick={() => setTagToDelete(null)}
                              >
                                Cancel
                              </AlertDialogCancel>
                              <AlertDialogAction onClick={handleDeleteTag}>
                                Continue
                              </AlertDialogAction>
                            </AlertDialogFooter>
                          </AlertDialogContent>
                        </AlertDialog>
                        <AlertDialog>
                          <AlertDialogTrigger asChild>
                            <Button
                              variant="outline"
                              size="sm"
                              onClick={() => setTagToTakeOwnership(tag)}
                            >
                              Take Ownership
                            </Button>
                          </AlertDialogTrigger>
                          <AlertDialogContent>
                            <AlertDialogHeader>
                              <AlertDialogTitle>
                                Take Ownership?
                              </AlertDialogTitle>
                              <AlertDialogDescription>
                                Are you sure you want to take ownership of tag "
                                {tag.name}"?
                              </AlertDialogDescription>
                            </AlertDialogHeader>
                            <AlertDialogFooter>
                              <AlertDialogCancel
                                onClick={() => setTagToTakeOwnership(null)}
                              >
                                Cancel
                              </AlertDialogCancel>
                              <AlertDialogAction onClick={handleTakeOwnership}>
                                Continue
                              </AlertDialogAction>
                            </AlertDialogFooter>
                          </AlertDialogContent>
                        </AlertDialog>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
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
                Page {page} of {pageCount}
              </span>
              <Button
                onClick={() => setPage(page + 1)}
                disabled={page === pageCount}
                variant="outline"
              >
                Next
              </Button>
              <Button
                onClick={() => setPage(pageCount)}
                disabled={page === pageCount}
                variant="outline"
              >
                Last
              </Button>
            </div>
          </CardContent>
        </Card>
      </DashboardLayout>
    </>
  );
}
