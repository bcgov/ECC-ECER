#!/usr/bin/env node

import fs from "fs";
import path from "path";

interface ColorMap {
  [key: string]: string;
}

function extractColorsFromTheme(filePath: string): ColorMap {
  const content = fs.readFileSync(filePath, "utf-8");
  const colors: ColorMap = {};

  const colorMatches = content.match(/(["']?)([^"'\s]+)\1\s*:\s*"#([A-Fa-f0-9]{6})"/g);

  if (colorMatches) {
    colorMatches.forEach((match) => {
      const [, , key, value] = match.match(/(["']?)([^"'\s]+)\1\s*:\s*"#([A-Fa-f0-9]{6})"/) || [];
      if (key && value) {
        colors[key] = value.toLowerCase();
      }
    });
  }

  return colors;
}

function extractColorsFromScss(filePath: string): ColorMap {
  const content = fs.readFileSync(filePath, "utf-8");
  const colors: ColorMap = {};

  // Extract colors from SCSS file
  const colorMatches = content.match(/\$([^:]+):\s*#([A-Fa-f0-9]{6})/g);

  if (colorMatches) {
    colorMatches.forEach((match) => {
      const [, key, value] = match.match(/\$([^:]+):\s*#([A-Fa-f0-9]{6})/) || [];
      if (key && value) {
        colors[key] = value.toLowerCase();
      }
    });
  }

  return colors;
}

function compareColors(
  themeColors: ColorMap,
  scssColors: ColorMap,
): {
  missingInScss: string[];
  missingInTheme: string[];
  mismatched: Array<{ key: string; themeValue: string; scssValue: string }>;
} {
  const missingInScss: string[] = [];
  const missingInTheme: string[] = [];
  const mismatched: Array<{ key: string; themeValue: string; scssValue: string }> = [];

  // Check for colors in theme but not in SCSS
  Object.keys(themeColors).forEach((key) => {
    if (!scssColors[key]) {
      missingInScss.push(key);
    } else if (themeColors[key] !== scssColors[key]) {
      mismatched.push({
        key,
        themeValue: themeColors[key],
        scssValue: scssColors[key],
      });
    }
  });

  // Check for colors in SCSS but not in theme
  Object.keys(scssColors).forEach((key) => {
    if (!themeColors[key]) {
      missingInTheme.push(key);
    }
  });

  return { missingInScss, missingInTheme, mismatched };
}

function main() {
  const themePath = path.join(process.cwd(), "src/styles/ecer-theme.ts");
  const scssPath = path.join(process.cwd(), "src/styles/_colors.scss");

  if (!fs.existsSync(themePath)) {
    console.error("❌ Theme file not found:", themePath);
    process.exit(1);
  }

  if (!fs.existsSync(scssPath)) {
    console.error("❌ SCSS colors file not found:", scssPath);
    process.exit(1);
  }

  console.log("🔍 Validating color consistency...\n");

  const themeColors = extractColorsFromTheme(themePath);
  const scssColors = extractColorsFromScss(scssPath);

  console.log(`📊 Found ${Object.keys(themeColors).length} colors in theme file`);
  console.log(`📊 Found ${Object.keys(scssColors).length} colors in SCSS file\n`);

  const comparison = compareColors(themeColors, scssColors);

  let hasErrors = false;

  if (comparison.missingInScss.length > 0) {
    console.log("❌ Colors missing in SCSS file:");
    comparison.missingInScss.forEach((color) => {
      console.log(`   - ${color}: #${themeColors[color]}`);
    });
    console.log("");
    hasErrors = true;
  }

  if (comparison.missingInTheme.length > 0) {
    console.log("❌ Colors missing in theme file:");
    comparison.missingInTheme.forEach((color) => {
      console.log(`   - ${color}: #${scssColors[color]}`);
    });
    console.log("");
    hasErrors = true;
  }

  if (comparison.mismatched.length > 0) {
    console.log("❌ Colors with mismatched values:");
    comparison.mismatched.forEach(({ key, themeValue, scssValue }) => {
      console.log(`   - ${key}: theme=#${themeValue}, scss=#${scssValue}`);
    });
    console.log("");
    hasErrors = true;
  }

  if (hasErrors) {
    console.log("💡 To fix these issues:");
    console.log("   1. Add missing colors to the appropriate file");
    console.log("   2. Update mismatched color values to match");
    console.log("   3. Run this script again to verify\n");
    process.exit(1);
  } else {
    console.log("✅ All colors are consistent between theme and SCSS files!");
  }
}

if (require.main === module) {
  main();
}
