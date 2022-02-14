module.exports = {
  root: true,
  env: {
    es2022: true,
    node: true,
    browser: true,
    "vue/setup-compiler-macros": true,
  },
  extends: ["eslint:recommended", "plugin:vue/vue3-recommended", "prettier"],
  parser: "vue-eslint-parser",
  parserOptions: {
    parser: "@typescript-eslint/parser",
  },
  plugins: ["@typescript-eslint"],
  rules: {
    "no-console": process.env.MODE === "production" ? "warn" : "off",
    "no-debugger": process.env.MODE === "production" ? "warn" : "off",
  },
};
