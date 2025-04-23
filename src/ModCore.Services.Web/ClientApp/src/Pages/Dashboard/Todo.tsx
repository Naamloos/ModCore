import { Head } from "@inertiajs/react";
import DashboardLayout from "@/Layouts/DashboardLayout";

export default function Todo() {
  return (
    <>
      <Head title="Welcome" />

      <DashboardLayout>
        <h1 className="text-center text-3xl font-extrabold tracking-tight text-white">
          Coming soon!
        </h1>
        <p className="text-center text-white mt-4">
          This module has not yet been implemented! Please check back later.
        </p>
        <p className="text-center text-white mt-4">
          <a
            onClick={() => window.history.back()}
            className="text-blue-400 hover:text-blue-300 cursor-pointer"
          >
            Return to previous page
          </a>
        </p>
      </DashboardLayout>
    </>
  );
}
