import "./bootstrap";
import "./app.css";
import React from "react";
import { createRoot } from "react-dom/client";
import { createInertiaApp } from "@inertiajs/react";
import { resolvePageComponent } from "laravel-vite-plugin/inertia-helpers";

const appTitle = import.meta.env.VITE_APP_TITLE;

createInertiaApp({
  title: (title) => title ? `${title} - ${appTitle}` : appTitle,
  resolve: (name) =>
    resolvePageComponent(
      `./Pages/${name}.jsx`,
      import.meta.glob("./Pages/**/*.jsx")
    ),

  setup({ el, App, props }) {
    const root = createRoot(el);
    root.render(<App {...props} />);
  },
});
