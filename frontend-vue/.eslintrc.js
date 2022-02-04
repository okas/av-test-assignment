module.exports = {
  root: true,
  env: {
    es2022: true,//es2021 can be found highest sepcific version in docs
  },
  extends: [
    "plugin:vue/vue3-strongly-recommended",
    "eslint:recommended",
    "@vue/prettier",
  ],
  parserOptions: {
    //ecmaVersion: 2022,
  },
  rules: {
    "no-console": import.meta.env.MODE === "production" ? "warn" : "off",
    "no-debugger": import.meta.env.MODE === "production" ? "warn" : "off",
  },
};
