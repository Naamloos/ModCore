/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    //"./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        gray: {
          50: '#f8f9fa',
          100: '#f1f3f5',
          200: '#e9ecef',
          300: '#dee2e6',
          400: '#ced4da',
          500: '#adb5bd',
          600: '#6c757d',
          700: '#495057',
          800: '#343a40',
          900: '#212529',
        },
        blue: {
          50: '#f5f9ff',
          100: '#e8f0ff',
          200: '#c7d8ff',
          300: '#a3bfff',
          400: '#7fa6ff',
          500: '#5b8dff',
          600: '#3f74e0',
          700: '#2f5bb3',
          800: '#204286',
          900: '#102959',
        },
        red: {
          50: '#ffe5e5',
          100: '#ffb8b8',
          200: '#ff8a8a',
          300: '#ff5c5c',
          400: '#ff2e2e',
          500: '#ff0000',
          600: '#e00000',
          700: '#b30000',
          800: '#860000',
          900: '#590000',
        },
      },
    },
  },
  plugins: [],
}

