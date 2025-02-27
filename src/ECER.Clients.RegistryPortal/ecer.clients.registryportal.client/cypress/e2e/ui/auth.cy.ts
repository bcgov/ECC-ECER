// Define the DeviceConfig interface.
interface DeviceConfig {
  device: string;
  width: number;
  height: number;
}

// Default device configurations.
const defaultDevices: DeviceConfig[] = [
  { device: "Desktop", width: 1280, height: 720 },
  { device: "Mobile", width: 375, height: 667 },
  { device: "Tablet", width: 768, height: 1024 },
];

// Compute devices synchronously using Cypress.env.
// If the environment variable "DEVICES" is provided, parse it; otherwise, use the defaults.
let devices: DeviceConfig[];
try {
  const devicesEnv = Cypress.env("DEVICES");
  devices = devicesEnv ? (JSON.parse(devicesEnv) as DeviceConfig[]) : defaultDevices;
} catch (error) {
  devices = defaultDevices;
}

// Precompute test cases outside of the describe block.
const testCases = devices.map(({ device, width, height }) => ({
  title: `Testing on ${device} (${width}x${height})`,
  device,
  width,
  height,
}));

describe("Authentication Tests across Multiple Devices", () => {
  for (const test of testCases) {
    describe(test.title, () => {
      beforeEach(() => {
        // Reset cookies and local storage (cy.resetState() is defined in custom commands)
        cy.resetState();
        // Set the viewport for the current device.
        cy.viewport(test.width, test.height);
        // Visit the base URL (configured in your cypress.config.ts).
        cy.visit("/login");
        cy.document().its("readyState").should("eq", "complete");
      });

      it(`should log in successfully on ${test.device}`, () => {
        cy.login();
        // Check if the profile page is available.
        cy.visit("/profile");
        cy.document().its("readyState").should("eq", "complete");
        cy.contains("Email address", { timeout: 10000 }).should("be.visible");
      });
    });
  }
});
