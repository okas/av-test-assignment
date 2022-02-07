module.exports = {
  root: true,
  env: {
    es2021: true,
  },
  extends: ["eslint:recommended", "plugin:vue/vue3-recommended", "prettier"],
  parserOptions: {
    ecmaVersion: 2022,
  },
  rules: {
    "no-console": process.env.MODE === "production" ? "warn" : "off",
    "no-debugger": process.env.MODE === "production" ? "warn" : "off",
  },
};
