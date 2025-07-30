import selectors from "./selectors";

Cypress.Commands.add("ECEAssistantWorkflow", (provinceName, certificationName) => {
  /** Contact Information */
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

  cy.contains("Transfer to: ECE Assistant").should("be.visible");
  cy.contains(provinceName).should("exist");
  cy.contains(certificationName).should("exist");

  cy.get(selectors.applicationPreview.characterReferenceFirstName).should("be.visible").should("contain.text", "CharacterReferenceFirstName");
  cy.get(selectors.applicationPreview.characterReferenceLastName).should("be.visible").should("contain.text", "CharacterReferenceLastName");
  cy.get(selectors.applicationPreview.characterReferenceEmail).should("be.visible").should("contain.text", "Character_Reference@test.gov.bc.ca");

  cy.get(selectors.applicationWizard.submitApplicationButton).click();

  cy.ApplicationSubmitted();
});
Cypress.Commands.add("ECEOneYearWorkflow", (provinceName, certificationName) => {
  /** Contact Information */
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

  cy.contains("Transfer to: ECE One Year").should("be.visible");
  cy.contains(provinceName).should("exist");
  cy.contains(certificationName).should("exist");

  cy.get(selectors.applicationPreview.characterReferenceFirstName).should("be.visible").should("contain.text", "CharacterReferenceFirstName");
  cy.get(selectors.applicationPreview.characterReferenceLastName).should("be.visible").should("contain.text", "CharacterReferenceLastName");
  cy.get(selectors.applicationPreview.characterReferenceEmail).should("be.visible").should("contain.text", "Character_Reference@test.gov.bc.ca");

  cy.get(selectors.applicationWizard.submitApplicationButton).click();

  cy.ApplicationSubmitted();
});

Cypress.Commands.add("ECEFiveYearWorkflow", (provinceName, certificationName) => {
  /** Contact Information */
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
  cy.get(selectors.workExperienceReference.phoneNumberInput).type("1234567890");
  cy.get(selectors.workExperienceReference.hoursInput).type("500");

  cy.get(selectors.workExperienceReference.saveReferenceButton).click();

  cy.get(selectors.applicationWizard.saveAndContinueButton).click();

  /** Application Review and Submit */
  cy.document().its("readyState").should("eq", "complete");

  cy.contains("Review and submit").should("be.visible");

  cy.contains("Transfer to: ECE Five Year").should("be.visible");
  cy.contains(provinceName).should("exist");
  cy.contains(certificationName).should("exist");

  cy.get(selectors.applicationPreview.characterReferenceFirstName).should("be.visible").should("contain.text", "CharacterReferenceFirstName");
  cy.get(selectors.applicationPreview.characterReferenceLastName).should("be.visible").should("contain.text", "CharacterReferenceLastName");
  cy.get(selectors.applicationPreview.characterReferenceEmail).should("be.visible").should("contain.text", "Character_Reference@test.gov.bc.ca");

  cy.get(selectors.applicationPreview.workReferenceName).should("be.visible").should("contain.text", "WorkReferenceFirstName");
  cy.get(selectors.applicationPreview.workReferenceName).should("be.visible").should("contain.text", "WorkReferenceLastName");
  cy.get(selectors.applicationPreview.workReferenceEmail).should("be.visible").should("contain.text", "WorkExperience_Reference@test.gov.bc.ca");

  cy.get(selectors.applicationWizard.submitApplicationButton).click();

  cy.ApplicationSubmitted();
});
Cypress.Commands.add("ApplicationSubmitted", () => {
  /** Application Submitted */
  cy.document().its("readyState").should("eq", "complete");
  cy.get(selectors.applicationSubmitted.pageTitle).should("be.visible").should("contain.text", "Application Submitted");
  cy.get(selectors.applicationSubmitted.applicationSummaryButton).should("be.visible").should("contain.text", "Go to application summary");
});
