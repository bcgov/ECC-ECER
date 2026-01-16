import selectors from "../../../support/selectors";
import { courseStartDay, courseEndDay } from "../../../support/utils";

describe("Active ECE 5 Year Certificate and Active ECE one Year but ECE 5 year is visible  Application", () => {
  it("Active 5 year ECE cetification should be visible", () => {
    cy.seedRenewalApplication("ECE5Years", false, false);

    cy.reload();
    /** Dashboard */
    cy.seedRenewalApplication("ECEOneYear", false, false);

    cy.reload();
    cy.contains("Early Childhood Educator - ECE Five Year").should("be.visible");
  });
});
