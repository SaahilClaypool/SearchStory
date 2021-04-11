// Example `tailwind.config.js` file
const colors = require('tailwindcss/colors')

module.exports = {
  purge: {
    enabled: true,
    content: [
      "../Pages/**/*.{html,razor,cshtml}",
      "../Pages/*.{html,razor,cshtml}",
      "../Shared/**/*.{html,razor,cshtml}",
      "../Shared/*.{html,razor,cshtml}",
    ]
  },
  theme: {
    extend: {
      spacing: {
        '128': '32rem',
        '144': '36rem',
      },
      borderRadius: {
        '4xl': '2rem',
      }
    }
  },
  variants: {
    extend: {
      borderColor: ['focus-visible'],
      opacity: ['disabled'],
    }
  }
}