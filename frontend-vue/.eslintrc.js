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
    "no-console": process.env.NODE_ENV === "production" ? "warn" : "off",
    "no-debugger": process.env.NODE_ENV === "production" ? "warn" : "off",
  },
};
