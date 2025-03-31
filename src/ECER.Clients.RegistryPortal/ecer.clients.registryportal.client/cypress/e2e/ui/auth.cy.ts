// Define the DeviceConfig interface.
interface DeviceConfig {
  device: string;
  width: number;
  height: number;
}

// Compute devices synchronously using Cypress.env.
// If the environment variable "DEVICES" is provided, parse it; otherwise, use the defaults.
let devices: DeviceConfig[];
try {
  const devicesEnv = Cypress.env("DEVICES");
  devices = JSON.parse(devicesEnv) as DeviceConfig[];
} catch (error) {
  devices = [];
}

// Precompute test cases outside of the describe block.
const testCases = devices.map(({ device, width, height }) => ({
  title: `Testing on ${device} (${width}x${height})`,
  device,
  width,
  height,
}));

describe("BCSC Authentication Tests", () => {
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
        cy.contains("Email address").should("be.visible");
      });
    });
  }
});
