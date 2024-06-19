describe('look-at-comment', () => {
  beforeEach(() => {
    cy.visit('https://rankingforum.lightai.dev/rankingforumtest')
  })

  it('Open MapDiscussion', () => {
    cy.get('.difficultyOption').click()
  })
})