// This script sets up certificate for HTTPS for the application using the ASP.NET Core HTTPS certificate
import { existsSync } from "fs";
import { spawn } from "child_process";
import { certFilePath, keyFilePath } from "./https.dev.config.js";

if (!existsSync(certFilePath) || !existsSync(keyFilePath)) {
  spawn(
    "dotnet",
    [
      "dev-certs",
      "https",
      "--export-path",
      certFilePath,
      "--format",
      "Pem",
      "--no-password",
    ],
    { stdio: "inherit" }
  ).on("exit", (code) => process.exit(code));
}
