import selectors from "../../../../support/selectors";

describe("Renew Active ECE One Year Certificate Application", () => {
  it("should create a sucessfull Renewal - ECE One Year Application", () => {
    cy.seedRenewalApplication("ECEOneYear", true, false);

    cy.reload();
    /** Dashboard */
    cy.get(selectors.dashboard.renew).click();

    /** Application Requirements */
    cy.get(selectors.applicationRequirements.applyNowButton).click();

    /** Declaration */
    cy.get(selectors.declaration.declarationCheckbox).check({ force: true });
    cy.get(selectors.declaration.continueButton).click();

    /** Contact Information */
    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    /** Renewal Information */
    cy.get(selectors.renewalInformation.oneYearRenewalInformationRadio).check();
    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    /** Character Reference */
    cy.get(selectors.characterReference.lastNameInput).type("CharacterReferenceLastName");
    cy.get(selectors.characterReference.firstNameInput).type("CharacterReferenceFirstName");
    cy.get(selectors.characterReference.emailInput).type("Character_Reference@test.gov.bc.ca");
    cy.get(selectors.characterReference.phoneNumberInput).type("1234567890");

    cy.get(selectors.applicationWizard.saveAndContinueButton).click();

    /** Application Review and Submit */
    cy.document().its("readyState").should("eq", "complete");
    cy.contains("Review and submit").should("be.visible");
    cy.get(selectors.applicationPreview.certificationType).should("be.visible").should("contain.text", "ECE One Year");
    cy.get(selectors.applicationPreview.characterReferenceFirstName).should("be.visible").should("contain.text", "CharacterReferenceFirstName");
    cy.get(selectors.applicationPreview.characterReferenceLastName).should("be.visible").should("contain.text", "CharacterReferenceLastName");
    cy.get(selectors.applicationPreview.characterReferenceEmail).should("be.visible").should("contain.text", "Character_Reference@test.gov.bc.ca");
    cy.get(selectors.applicationWizard.submitApplicationButton).click();

    /** Application Submitted */
    cy.document().its("readyState").should("eq", "complete");
    cy.get(selectors.applicationSubmitted.pageTitle).should("be.visible").should("contain.text", "Application Submitted");
    cy.get(selectors.applicationSubmitted.applicationSummaryButton).should("be.visible").should("contain.text", "Go to application summary");
  });
});
