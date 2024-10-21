import typescript from "@typescript-eslint/eslint-plugin";
import prettier from "eslint-plugin-prettier";
import simpleImportSort from "eslint-plugin-simple-import-sort";
import unusedImports from "eslint-plugin-unused-imports";
import vue from "eslint-plugin-vue";

export default [
  {
    files: ["*.vue", "*.js", "*.jsx", "*.cjs", "*.mjs", "*.ts", "*.tsx", "*.cts", "*.mts"],
    ignores: [".gitignore"], // Ignore files as per your ignore patterns

    languageOptions: {
      parserOptions: {
        ecmaVersion: "latest",
        sourceType: "module",
      },
      globals: {
        // Define global variables here if needed
      },
    },

    plugins: {
      vue,
      typescript,
      prettier,
      "simple-import-sort": simpleImportSort,
      "unused-imports": unusedImports,
    },

    rules: {
      "prettier/prettier": ["error", { endOfLine: "auto" }],
      "@typescript-eslint/no-unused-vars": "off",
      "unused-imports/no-unused-imports": "warn",
      "unused-imports/no-unused-vars": ["warn", { vars: "all", args: "after-used", argsIgnorePattern: "^_" }],
      "simple-import-sort/imports": "error",
      "simple-import-sort/exports": "error",
      "vue/multi-word-component-names": "off",
    },

    linterOptions: {
      // Optionally configure linter-specific options here
    },
  },
];
