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

describe("New ECE 5 Year + ITE & SNE Certificate Application", () => {
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

        //Reset User State
        cy.resetUserState();
      });

      it(`should sucessfully create a New ECE 5 Year + ITE & SNE Application on ${test.device}`, () => {
        const today: Date = new Date();
        const day = today.getDate();

        cy.login();
        /** Dashboard */
        cy.get(selectors.dashboard.applyNowButton).click();

        /** Certification Type */
        cy.get(selectors.certificationType.eceFiveYearRadio).check();
        cy.get(selectors.certificationType.iteCheckBox).check({ force: true });
        cy.get(selectors.certificationType.sneCheckBox).check({ force: true });
        cy.get(selectors.certificationType.continueButton).click();

        /** Application Requirements */
        cy.get(selectors.applicationRequirements.applyNowButton).click();

        /** Declaration */
        cy.get(selectors.declaration.declarationCheckbox).check({ force: true });
        cy.get(selectors.declaration.continueButton).click();

        /** Contact Information */
        cy.get(selectors.applicationWizard.saveAndContinueButton).click();

        /** Education */
        cy.get(selectors.education.addEducationButton).click();

        cy.get(selectors.education.transcriptStatusRadioDiv).within(() => {
          cy.get(selectors.elementType.radio).first().check();
        });
        cy.get(selectors.education.programNameInput).type("TEST ECE 5 Year Course");

        /* Start Date - DatePicker*/
        cy.get(selectors.education.programStartDateInput).click({ force: true });
        Cypress._.times(7, () => {
          cy.get(selectors.datePicker.prevMonthButton).first().click();
        });
        cy.get(selectors.datePicker.monthDiv)
          .first()
          .should("exist")
          .within(() => {
            cy.contains("span", `${day}`).click({ force: true });
          });
        cy.get("button").contains("OK").click({ force: true });

        /* End Date - DatePicker*/
        cy.get(selectors.education.programEndDateInput).click({ force: true });
        Cypress._.times(1, () => {
          cy.get(selectors.datePicker.prevMonthButton).first().click();
        });
        cy.get(selectors.datePicker.monthDiv)
          .first()
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
        cy.get(selectors.education.saveEducationButton).click();

        cy.get(selectors.applicationWizard.saveAndContinueButton).click();

        /** Character Reference */
        cy.get(selectors.characterReference.lastNameInput).type("CharacterReferenceLastName");
        cy.get(selectors.characterReference.firstNameInput).type("CharacterReferenceFirstName");
        cy.get(selectors.characterReference.emailInput).type("Character_Reference@test.gov.bc.ca");
        cy.get(selectors.characterReference.phoneNumberInput).type("1234567890");

        cy.get(selectors.applicationWizard.saveAndContinueButton).click();

        /** Work Experience Reference */
        cy.document().its("readyState").should("eq", "complete");
        cy.get(selectors.workExperienceReference.addReferenceButton).click();

        cy.get(selectors.workExperienceReference.lastNameInput).type("WorkReferenceLastName");
        cy.get(selectors.workExperienceReference.firstNameInput).type("WorkReferenceFirstName");
        cy.get(selectors.workExperienceReference.emailInput).type("WorkExperience_Reference@test.gov.bc.ca");
        cy.get(selectors.workExperienceReference.phoneNumberInput).type("1234567890", { force: true });
        cy.get(selectors.workExperienceReference.hoursInput).type("500", { force: true });

        cy.get(selectors.workExperienceReference.saveReferenceButton).click();

        cy.get(selectors.applicationWizard.saveAndContinueButton).click();

        /** Application Review and Submit */
        cy.document().its("readyState").should("eq", "complete");

        cy.contains("Review and submit").should("be.visible");
        cy.get(selectors.applicationPreview.certificationType).should("be.visible").should("contain.text", "ECE Five Year");
        cy.get(selectors.applicationPreview.characterReferenceFirstName).should("be.visible").should("contain.text", "CharacterReferenceFirstName");
        cy.get(selectors.applicationPreview.characterReferenceLastName).should("be.visible").should("contain.text", "CharacterReferenceLastName");
        cy.get(selectors.applicationPreview.characterReferenceEmail).should("be.visible").should("contain.text", "Character_Reference@test.gov.bc.ca");
        cy.get(selectors.applicationPreview.educationCountry).should("be.visible").should("contain.text", "Canada");
        cy.get(selectors.applicationPreview.educationProvince).should("be.visible").should("contain.text", "British Columbia");

        cy.get(selectors.applicationPreview.workReferenceName).should("be.visible").should("contain.text", "WorkReferenceFirstName");
        cy.get(selectors.applicationPreview.workReferenceName).should("be.visible").should("contain.text", "WorkReferenceLastName");
        cy.get(selectors.applicationPreview.workReferenceEmail).should("be.visible").should("contain.text", "WorkExperience_Reference@test.gov.bc.ca");

        cy.get(selectors.applicationWizard.submitApplicationButton).click();

        /** Application Submitted */
        cy.document().its("readyState").should("eq", "complete");
        cy.get(selectors.applicationSubmitted.pageTitle).should("be.visible").should("contain.text", "Application Submitted");
        cy.get(selectors.applicationSubmitted.applicationSummaryButton).should("be.visible").should("contain.text", "Go to application summary");
      });
    });
  }
});
