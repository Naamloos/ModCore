import DashboardLayout from "@/Layouts/DashboardLayout";
import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { PagePropsWith } from "@/Types/PageProps";
import { Button } from "@/Components/ui/button";
import { useState, useEffect } from "react";
import { Input } from "@/Components/ui/input";
import { useToast } from "@/hooks/use-toast";
import { router } from "@inertiajs/react";
import ModCoreBanAppeal from "@/Types/DatabaseTypes/ModCoreBanAppeal";

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

interface ConfigureBanAppealsProps {
  server: DiscordGuild;
  appeals: ModCoreBanAppeal[];
}

export default function Configure({
  user,
  server,
  appeals: initialAppeals,
}: PagePropsWith<ConfigureBanAppealsProps>) {
  let icon = server?.icon ? server.icon : null;
  if (icon) {
    const ext = icon.includes("a_") ? ".gif" : ".png";
    icon = "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
  } else {
    icon = "https://cdn.discordapp.com/embed/avatars/0.png";
  }

  const [appeals, setAppeals] = useState<ModCoreBanAppeal[]>(initialAppeals);
  const [page, setPage] = useState(1);
  const itemsPerPage = 5;
  const [search, setSearch] = useState("");
  const [filteredAppeals, setFilteredAppeals] =
    useState<ModCoreBanAppeal[]>(initialAppeals);
  const [appealToAction, setAppealToAction] = useState<{
    appeal: ModCoreBanAppeal;
    action: string;
  } | null>(null);
  const { toast } = useToast();

  useEffect(() => {
    const results = initialAppeals.filter((appeal) => {
      return (
        appeal.user_id.toLowerCase().includes(search.toLowerCase()) ||
        appeal.appeal_content.toLowerCase().includes(search.toLowerCase())
      );
    });
    setFilteredAppeals(results);
    setPage(1); // Reset to first page when search changes
  }, [search, initialAppeals]);

  const pageCount = Math.ceil(filteredAppeals.length / itemsPerPage);
  const paginatedAppeals = filteredAppeals.slice(
    (page - 1) * itemsPerPage,
    page * itemsPerPage,
  );

  const handleApproveAppeal = () => {
    if (appealToAction && appealToAction.action === "approve") {
      // Implement your approve appeal logic here, e.g., API call
      const updatedAppeal = {
        ...appealToAction.appeal,
        status: "approved" as const,
        resolved_at: new Date().toISOString(),
        resolved_by: user?.id?.toString(),
      };

      setAppeals(
        appeals.map((appeal) =>
          appeal.user_id === updatedAppeal.user_id &&
          appeal.guild_id === updatedAppeal.guild_id
            ? updatedAppeal
            : appeal,
        ),
      );

      setFilteredAppeals(
        filteredAppeals.map((appeal) =>
          appeal.user_id === updatedAppeal.user_id &&
          appeal.guild_id === updatedAppeal.guild_id
            ? updatedAppeal
            : appeal,
        ),
      );

      setAppealToAction(null);
      toast({
        title: "Appeal Approved",
        description: `Ban appeal from user ${updatedAppeal.user_id} has been approved.`,
      });
    }
  };

  const handleDenyAppeal = () => {
    if (appealToAction && appealToAction.action === "deny") {
      // Implement your deny appeal logic here, e.g., API call
      const updatedAppeal = {
        ...appealToAction.appeal,
        status: "denied" as const,
        resolved_at: new Date().toISOString(),
        resolved_by: user?.id?.toString(),
      };

      setAppeals(
        appeals.map((appeal) =>
          appeal.user_id === updatedAppeal.user_id &&
          appeal.guild_id === updatedAppeal.guild_id
            ? updatedAppeal
            : appeal,
        ),
      );

      setFilteredAppeals(
        filteredAppeals.map((appeal) =>
          appeal.user_id === updatedAppeal.user_id &&
          appeal.guild_id === updatedAppeal.guild_id
            ? updatedAppeal
            : appeal,
        ),
      );

      setAppealToAction(null);
      toast({
        title: "Appeal Denied",
        description: `Ban appeal from user ${updatedAppeal.user_id} has been denied.`,
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
            {server.name}: Ban Appeal Management
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
                <CardTitle className="text-2xl">Ban Appeals</CardTitle>
                <CardDescription>
                  Manage ban appeals for this server.
                </CardDescription>
              </div>
              <Input
                type="search"
                placeholder="Search appeals..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="max-w-md"
              />
            </div>
          </CardHeader>
          <CardContent>
            <Table>
              <TableCaption>
                A list of all ban appeals for this server.
              </TableCaption>
              <TableHeader>
                <TableRow>
                  <TableHead className="w-[100px]">User ID</TableHead>
                  <TableHead>Appeal Excerpt</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {paginatedAppeals.map((appeal) => (
                  <TableRow key={appeal.user_id}>
                    <TableCell className="font-medium">
                      {appeal.user_id}
                    </TableCell>
                    <TableCell>
                      {appeal.appeal_content.length > 50
                        ? appeal.appeal_content.substring(0, 50) + "..."
                        : appeal.appeal_content}
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
                              <DialogTitle>Ban Appeal</DialogTitle>
                              <DialogDescription>
                                Submitted by user ID: {appeal.user_id}
                              </DialogDescription>
                            </DialogHeader>
                            <div className="mt-4">
                              <h4 className="font-medium text-sm mb-2">
                                Appeal Content:
                              </h4>
                              <div className="p-4 bg-gray-800 rounded-md whitespace-pre-wrap">
                                {appeal.appeal_content}
                              </div>
                            </div>
                          </DialogContent>
                        </Dialog>

                        <>
                          <AlertDialog>
                            <AlertDialogTrigger asChild>
                              <Button
                                variant="default"
                                size="sm"
                                className="bg-green-600 hover:bg-green-700"
                                onClick={() =>
                                  setAppealToAction({
                                    appeal,
                                    action: "approve",
                                  })
                                }
                              >
                                Approve
                              </Button>
                            </AlertDialogTrigger>
                            <AlertDialogContent>
                              <AlertDialogHeader>
                                <AlertDialogTitle>
                                  Approve Ban Appeal?
                                </AlertDialogTitle>
                                <AlertDialogDescription>
                                  Are you sure you want to approve this ban
                                  appeal? This will mark the appeal as approved
                                  and notify the user, if possible.
                                </AlertDialogDescription>
                              </AlertDialogHeader>
                              <AlertDialogFooter>
                                <AlertDialogCancel
                                  onClick={() => setAppealToAction(null)}
                                >
                                  Cancel
                                </AlertDialogCancel>
                                <AlertDialogAction
                                  onClick={handleApproveAppeal}
                                >
                                  Approve
                                </AlertDialogAction>
                              </AlertDialogFooter>
                            </AlertDialogContent>
                          </AlertDialog>

                          <AlertDialog>
                            <AlertDialogTrigger asChild>
                              <Button
                                variant="destructive"
                                size="sm"
                                onClick={() =>
                                  setAppealToAction({
                                    appeal,
                                    action: "deny",
                                  })
                                }
                              >
                                Deny
                              </Button>
                            </AlertDialogTrigger>
                            <AlertDialogContent>
                              <AlertDialogHeader>
                                <AlertDialogTitle>
                                  Deny Ban Appeal?
                                </AlertDialogTitle>
                                <AlertDialogDescription>
                                  Are you sure you want to deny this ban appeal?
                                  This will mark the appeal as denied.
                                </AlertDialogDescription>
                              </AlertDialogHeader>
                              <AlertDialogFooter>
                                <AlertDialogCancel
                                  onClick={() => setAppealToAction(null)}
                                >
                                  Cancel
                                </AlertDialogCancel>
                                <AlertDialogAction onClick={handleDenyAppeal}>
                                  Deny
                                </AlertDialogAction>
                              </AlertDialogFooter>
                            </AlertDialogContent>
                          </AlertDialog>
                        </>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
                {paginatedAppeals.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={5} className="text-center py-8">
                      No ban appeals found.
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
            {filteredAppeals.length > 0 && (
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
            )}
          </CardContent>
        </Card>
      </DashboardLayout>
    </>
  );
}
