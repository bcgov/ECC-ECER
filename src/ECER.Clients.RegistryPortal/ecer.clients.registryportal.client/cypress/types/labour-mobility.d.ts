declare global {
  namespace Cypress {
    interface Chainable {
      ECEAssistantWorkflow(
        provinceName: string,
        certificationName: string,
      ): Chainable<void>;
      ECEOneYearWorkflow(
        provinceName: string,
        certificationName: string,
      ): Chainable<void>;
      ECEFiveYearWorkflow(
        provinceName: string,
        certificationName: string,
      ): Chainable<void>;
      ApplicationSubmitted(): Chainable<void>;
    }
  }
}
export {};
