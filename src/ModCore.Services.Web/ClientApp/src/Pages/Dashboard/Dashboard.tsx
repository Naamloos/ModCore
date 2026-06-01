import { Head } from "@inertiajs/react";
import { PageProps } from "@/Types/PageProps";
import DashboardLayout from "@/Layouts/DashboardLayout";
import {
  IconChartLine,
  IconClock,
  IconDoorExit,
  IconHammer,
  IconPlus,
  IconTag,
  IconTicket,
} from "@tabler/icons-react";
import ModuleCard from "@/Components/ModuleCard";

// modules with name, description, link and icon (component)
// Your ban appeals, Your tickets, Your reminders, Your server levels
const modules = [
  {
    name: "Add to Server",
    description: "Add the ModCore to a new server.",
    link: "/dashboard/todo",
    icon: IconPlus,
  },
  {
    name: "Ban Appeals",
    description: "View statuses of your ban appeals.",
    link: "/dashboard/todo",
    icon: IconHammer,
  },
  {
    name: "Tickets",
    description: "View statuses of your support tickets.",
    link: "/dashboard/todo",
    icon: IconTicket,
  },
  {
    name: "Reminders",
    description: "Display your set reminders.",
    link: "/dashboard/todo",
    icon: IconClock,
  },
  {
    name: "Server Levels",
    description: "Display your server levels.",
    link: "/dashboard/todo",
    icon: IconChartLine,
  },
  {
    name: "Tags",
    description: "Manage your owned tags.",
    link: "/dashboard/todo",
    icon: IconTag,
  },
  {
    name: "Logout",
    description: "Logout from the ModCore Dashboard.",
    link: "/logout",
    icon: IconDoorExit,
    done: true,
  },
];

export default function Index({ user }: PageProps) {
  return (
    <>
      <Head title="Welcome" />

      <DashboardLayout>
        <div className="md:flex items-center mb-4">
          <img
            src={user!.avatar}
            className="inline-block h-16 w-16 rounded-full mr-4"
            alt="User Avatar"
          />
          <h1 className="text-3xl font-extrabold tracking-tight text-white py-2">
            Welcome, {user!.username}!
          </h1>
        </div>

        <div className="w-full">
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mt-4">
            {modules.map((module, index) => (
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
        </div>
      </DashboardLayout>
    </>
  );
}
