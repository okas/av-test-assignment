module.exports = {
  root: true,
  env: {
    es2021: true,
    node: true,
    browser: true,
    "vue/setup-compiler-macros": true,
  },
  extends: ["eslint:recommended", "plugin:vue/vue3-recommended", "prettier"],
  parser: "vue-eslint-parser",
  parserOptions: {
    ecmaVersion: 2022,
    parser: "@typescript-eslint/parser",
  },
  plugins: ["@typescript-eslint"],
  rules: {
    "no-console": process.env.MODE === "production" ? "warn" : "off",
    "no-debugger": process.env.MODE === "production" ? "warn" : "off",
  },
};
