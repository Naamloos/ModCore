import { User } from "@/Types/User";
import { router, usePage } from "@inertiajs/react";
import { IconMenu } from "@tabler/icons-react";
// @ts-expect-error This works.
import LogoImage from "@/Assets/logo.png";

export default function Navbar({
  isMenuOpen,
  setIsMenuOpen,
}: {
  isMenuOpen: boolean;
  setIsMenuOpen: (value: boolean) => void;
}) {
  const { user } = usePage<{
    user: User;
  }>().props;

  return (
    <>
      <div className="bg-gray-900 h-16 flex items-center justify-between px-4 border-b border-gray-800 w-full">
        <button
          className="text-gray-400 hover:text-white transition-colors md:hidden"
          onClick={() => setIsMenuOpen(!isMenuOpen)}
          type="button"
        >
          <div className="flex justify-center items-center h-6 w-6">
            <IconMenu size={24} />
          </div>
        </button>
        <div className="hidden md:block" />
        <div className="text-white font-medium flex items-center gap-2">
          <img
            src={LogoImage}
            className="h-10 w-10 inline-block"
            alt="Application Icon"
          />
        </div>
        <div className="flex items-center gap-4">
          <button
            className="w-12 h-12 mt-3 rounded-xl mb-3 hover:bg-gray-800 flex items-center justify-center text-gray-400 hover:text-white transition-colors"
            onClick={() => {
              router.visit("/dashboard");
            }}
            type="button"
          >
            <img
              src={user.avatar}
              alt="User Avatar"
              className="w-8 h-8 rounded-full"
            />
          </button>
        </div>
      </div>
    </>
  );
}
