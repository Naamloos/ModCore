import { Head, router } from "@inertiajs/react";
import { PagePropsWith } from "@/Types/PageProps";
import { DiscordGuild } from "@/Types/DiscordTypes/DiscordGuild";
import { ModCoreGuild } from "@/Types/DatabaseTypes/ModCoreGuild";
import DashboardLayout from "@/Layouts/DashboardLayout";
import {
  IconAlertSquareRounded,
  IconBook,
  IconChartLine,
  IconHammer,
  IconHandStop,
  IconJumpRope,
  IconMoodAngry,
  IconMoon,
  IconNotebook,
  IconRobot,
  IconStar,
  IconTag,
  IconTicket,
  IconTree,
} from "@tabler/icons-react";
import { hasPermissionsFromString } from "@/Types/DiscordTypes/DiscordPermission";
import ModuleCard from "@/Components/ModuleCard";
import { useState } from "react";
import { Button } from "../../../Components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "../../../Components/ui/dialog";

type ManagePageProps = {
  server: DiscordGuild;
  permissions: string[];
  databaseServer: ModCoreGuild;
};

// modules with name, description, link and icon (emoji)
const modules = (id: string) => [
  {
    name: "AutoRole",
    description: "Automatically assign roles to new members.",
    link: `/dashboard/servers/${id}/autorole`,
    icon: IconRobot,
    done: true,
  },
  {
    name: "Ban Appeal",
    description: "Allow members to appeal their bans.",
    link: `/dashboard/servers/${id}/banappeal`,
    icon: IconHammer,
    done: true,
  },
  {
    name: "Infractions",
    description: "Track and manage member infractions.",
    link: `/dashboard/servers/${id}/infractions`,
    icon: IconMoodAngry,
    done: true,
  },
  {
    name: "Levels",
    description: "Implement a leveling system for members.",
    link: `/dashboard/servers/${id}/leveling`,
    icon: IconChartLine,
    done: true,
  },
  {
    name: "Logging",
    description: "Log server events and activities.",
    link: `/dashboard/servers/${id}/logging`,
    icon: IconTree,
    done: true,
  },
  {
    name: "Profile States",
    description: "Manage member profile states.",
    link: `/dashboard/servers/${id}/profilestates`,
    icon: IconMoon,
    done: true,
  },
  {
    name: "Role Menus",
    description: "Create and manage role menus.",
    link: `/dashboard/servers/${id}/rolemenus`,
    icon: IconBook,
    done: true,
  },
  {
    name: "Starboards",
    description: "Set up starboards for starred messages.",
    link: `/dashboard/servers/${id}/starboards`,
    icon: IconStar,
    done: true,
  },
  {
    name: "Tags",
    description: "Create and manage custom tags.",
    link: `/dashboard/servers/${id}/tags`,
    icon: IconTag,
    done: true,
  },
  {
    name: "Tickets",
    description: "Manage support tickets.",
    link: `/dashboard/servers/${id}/tickets`,
    icon: IconTicket,
  },
  {
    name: "Welcome Messages",
    description: "Set up welcome messages for new members.",
    link: `/dashboard/servers/${id}/welcome`,
    icon: IconHandStop,
    done: true,
  },
  {
    name: "Nickname Approval",
    description: "Approve or reject member nicknames.",
    link: `/dashboard/todo`,
    icon: IconNotebook,
  },
  {
    name: "Jump Link Embed",
    description: "Embed jump links in messages.",
    link: `/dashboard/servers/${id}/jumplink`,
    icon: IconJumpRope,
    done: true,
  },
];

export default function Manage({ server }: PagePropsWith<ManagePageProps>) {
  if (!hasPermissionsFromString(server.permissions, "MANAGE_GUILD")) {
    router.visit("/login");
  }

  let icon = server?.icon ? server.icon : null;
  if (icon) {
    const ext = icon.includes("a_") ? ".gif" : ".png";
    icon = "https://cdn.discordapp.com/icons/" + server.id + "/" + icon + ext;
  } else {
    icon = "https://cdn.discordapp.com/embed/avatars/0.png";
  }

  const [resetModal, setResetModal] = useState(false);
  const [leaveModal, setLeaveModal] = useState(false);

  return (
    <>
      <Head title="Welcome" />

      <DashboardLayout>
        {server == null && (
          <>
            <h1 className="text-center text-3xl font-extrabold tracking-tight text-white">
              ModCore is not in this server!
            </h1>
            <p className="text-center text-white mt-4">
              Please contact the server owner to add ModCore.
            </p>
            <p className="text-center text-white mt-4">
              <a
                href="/dashboard/servers"
                className="text-blue-400 hover:text-blue-300"
              >
                Back to Servers
              </a>
            </p>
          </>
        )}
        {server && (
          <>
            <div className="md:flex items-center mb-4">
              <img
                src={icon}
                className="inline-block h-16 w-16 rounded-full mr-4"
                alt="User Avatar"
              />
              <h1 className="text-3xl font-extrabold tracking-tight text-white py-2">
                {server.name}
              </h1>
            </div>

            <div className="mt-4">
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mt-4">
                {modules(server.id).map((module, index) => (
                  <ModuleCard
                    key={index}
                    name={module.name}
                    description={module.description}
                    link={module.link}
                    icon={module.icon}
                    done={module.done ?? false}
                  />
                ))}
              </div>
              {/* Danger Zone, extra margin so it's not immediately visible */}
              <div className="border-red-900 border-solid p-4 mt-60 sm:mx-16 rounded-xl border bg-card text-card-foreground shadow">
                <h3 className="text-xl font-bold mb-4 text-center">
                  <IconAlertSquareRounded className="inline" />
                  &nbsp;Danger Zone&nbsp;
                  <IconAlertSquareRounded className="inline" />
                </h3>
                <p className="text-center mb-4">
                  These actions are irreversible! Proceed with caution!
                </p>
                <Dialog open={resetModal} onOpenChange={setResetModal}>
                  <DialogContent className="sm:max-w-[425px]">
                    <DialogHeader>
                      <DialogTitle>Reset Server</DialogTitle>
                      <DialogDescription>
                        Are you sure you want to reset ModCore in this server?
                        This will reset all settings and data associated with
                        ModCore in this server.
                      </DialogDescription>
                    </DialogHeader>
                    <div className="grid grid-cols-2 gap-4">
                      <Button
                        variant={"destructive"}
                        onClick={() => {
                          // Perform reset logic here
                          setResetModal(false);
                        }}
                      >
                        Reset Server
                      </Button>
                      <Button
                        type="button"
                        variant="secondary"
                        onClick={() => setResetModal(false)}
                      >
                        Cancel
                      </Button>
                    </div>
                  </DialogContent>
                </Dialog>

                <Dialog open={leaveModal} onOpenChange={setLeaveModal}>
                  <DialogContent className="sm:max-w-[425px]">
                    <DialogHeader>
                      <DialogTitle>Leave Server</DialogTitle>
                      <DialogDescription>
                        Are you sure you want to leave this server? This will
                        remove ModCore from this server and delete all
                        associated data.
                      </DialogDescription>
                    </DialogHeader>
                    <div className="grid grid-cols-2 gap-4">
                      <Button
                        variant={"destructive"}
                        onClick={() => {
                          // Perform leave server logic here
                          setLeaveModal(false);
                        }}
                      >
                        Leave Server
                      </Button>
                      <Button
                        type="button"
                        variant="secondary"
                        onClick={() => setLeaveModal(false)}
                      >
                        Cancel
                      </Button>
                    </div>
                  </DialogContent>
                </Dialog>
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <Button
                    variant={"destructive"}
                    onClick={() => setResetModal(true)}
                    className="py-2 px-4"
                  >
                    Reset Server
                  </Button>
                  <Button
                    variant={"destructive"}
                    onClick={() => setLeaveModal(true)}
                    className="py-2 px-4"
                  >
                    Leave Server
                  </Button>
                </div>
              </div>
            </div>
          </>
        )}
      </DashboardLayout>
    </>
  );
}
