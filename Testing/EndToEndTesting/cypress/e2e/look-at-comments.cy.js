describe("look-at-comments", () => {
  beforeEach(() => {
    cy.visit("https://rankingforum.lightai.dev/rankingforumtest");
  });

  it("Should open a MapDiscussion", () => {
    cy.get(".difficultyOption").click();
  });

  it("Opens General (This difficulty) comments", () => {
    cy.get(".difficultyOption").click();
    cy.get(".comment-sort-choices").children("div").eq(2).click();
  });

  it("Can see a General (This difficulty) comment", () => {
    cy.get(".difficultyOption").click();
    cy.get(".comment-sort-choices").children("div").eq(2).click();
    cy.get(".comment-box").should("exist");
  });

  it("Can see a Timeline comment", () => {
    cy.get(".difficultyOption").click();
    cy.get(".comment-sort-choices").children("div").eq(3).click();
    cy.get(".comment-box").should("exist");
  });
});
