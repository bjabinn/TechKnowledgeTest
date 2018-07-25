import { AgileMeterPage } from './app.po';

describe('scrum-meter App', function() {
  let page: AgileMeterPage;

  beforeEach(() => {
    page = new AgileMeterPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
