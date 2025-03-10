import selectors from "../../../support/selectors";

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

describe("New ECE Assistant Certificate Application", () => {
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

      it(`should create a sucessfull ECE Assistant Application on ${test.device}`, () => {
        const today: Date = new Date();
        const day = today.getDate();

        cy.login();
        /** Dashboard */
        cy.get(selectors.dashboard.applyNowButton).should("be.visible").click();

        /** Certification Type */
        cy.get(selectors.certificationType.eceAssistantRadio).check();
        cy.get(selectors.certificationType.continueButton).should("be.visible").click();

        /** Application Requirements */
        cy.get(selectors.applicationRequirements.applyNowButton).should("be.visible").click();

        /** Declaration */
        cy.get(selectors.declaration.declarationCheckbox).check({ force: true });
        cy.get(selectors.declaration.continueButton).should("be.visible").click();

        /** Contact Information */
        cy.get(selectors.applicationWizard.saveAndContinueButton, { timeout: 20000 }).should("be.visible").should("contain.text", "Save and continue").click();

        /** Education */
        cy.get(selectors.education.addEducationButton).should("be.visible").click();

        cy.get(selectors.education.transcriptStatusRadioDiv).within(() => {
          cy.get(selectors.elementType.radio).first().check();
        });
        cy.get(selectors.education.programNameInput).type("TEST ECE Assistant Course");

        cy.get(selectors.education.programStartDateInput).click({ force: true });

        cy.get(selectors.datePicker.monthDiv)
          .should("exist")
          .within(() => {
            cy.contains("span", `${day}`).click({ force: true });
          });

        cy.get("button").contains("OK").click({ force: true });

        cy.get(selectors.education.programEndDateInput).click({ force: true });
        cy.get(selectors.datePicker.monthDiv)
          .should("exist")
          .within(() => {
            cy.contains("span", `${day}`).click({ force: true });
          });

        cy.get("button").contains("OK").click({ force: true });

        cy.get(selectors.education.provinceDropDownList).should("exist").type("British Columbia", { force: true });

        cy.get(selectors.education.postSecondaryInstitutionDropDownList).should("exist").type("Other", { force: true });
        cy.get("body").click({ force: true });

        cy.get(selectors.education.institutionNameInput).type("TEST Educational Institution", { force: true });
        cy.get(selectors.education.studentIDInput).type("1234", { force: true });
        cy.get(selectors.education.nameOnTranscriptRadioDiv).within(() => {
          cy.get(selectors.elementType.radio).first().check();
        });
        cy.get(selectors.education.saveEducationButton).should("be.visible").click();

        cy.get(selectors.applicationWizard.saveAndContinueButton, { timeout: 20000 }).should("be.visible").should("contain.text", "Save and continue").click();

        /** Character Reference */
        cy.get(selectors.characterReference.lastNameInput).type("Reference Last Name");
        cy.get(selectors.characterReference.firstNameInput).type("Reference First Name");
        cy.get(selectors.characterReference.emailInput).type("Character_Reference@test.gov.bc.ca");
        cy.get(selectors.characterReference.phoneNumberInput).type("1234567890");

        cy.get(selectors.applicationWizard.saveAndContinueButton, { timeout: 20000 }).should("be.visible").should("contain.text", "Save and continue").click();

        /** Application Review and Submit */
        cy.document({ timeout: 10000 }).its("readyState").should("eq", "complete");
        cy.contains("Review and submit", { timeout: 10000 }).should("be.visible");
        cy.get(selectors.applicationPreview.certificationType).should("be.visible").should("contain.text", "ECE Assistant");
        cy.get(selectors.applicationPreview.characterReferenceFirstName).should("be.visible").should("contain.text", "Reference First Name");
        cy.get(selectors.applicationPreview.characterReferenceLastName).should("be.visible").should("contain.text", "Reference Last Name");
        cy.get(selectors.applicationPreview.characterReferenceEmail).should("be.visible").should("contain.text", "Character_Reference@test.gov.bc.ca");

        cy.get(selectors.applicationPreview.educationCountry).should("be.visible").should("contain.text", "Canada");

        cy.get(selectors.applicationPreview.educationProvince).should("be.visible").should("contain.text", "British Columbia");
        cy.get(selectors.applicationWizard.submitApplicationButton).should("be.visible").should("contain.text", "Submit application").click();

        /** Application Submitted */
        cy.document({ timeout: 10000 }).its("readyState").should("eq", "complete");
        cy.get(selectors.applicationSubmitted.pageTitle, { timeout: 10000 }).should("be.visible").should("contain.text", "Application Submitted");
        cy.get(selectors.applicationSubmitted.applicationSummaryButton, { timeout: 10000 })
          .should("be.visible")
          .should("contain.text", "Go to application summary");
      });
    });
  }
});
