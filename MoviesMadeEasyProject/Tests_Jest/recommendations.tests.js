/**
 * @jest-environment jsdom
 */
const { JSDOM } = require('jsdom');
require('@testing-library/jest-dom');
const { fireEvent } = require('@testing-library/dom');
require('jest-fetch-mock').enableMocks();

const {
  loadRecommendations,
  getMoreLikeThis,
  setupMoreLikeThisButtons
} = require("../MoviesMadeEasy/wwwroot/js/moreLikeThis");


beforeEach(() => {
  fetch.resetMocks();
  document.body.innerHTML = `
    <div id="loadingSpinner" style="display:none;"></div>
    <div id="results"></div>
    <div id="recommendationsContainer"></div>
    <div class="movie-card">
      <h5>The Hunger Games (2012)</h5>
      <button class="btn-outline-secondary">More Like This</button>
    </div>
  `;
  sessionStorage.clear();
});

describe('loadRecommendations', () => {
  test('renders warning if no recommendations in sessionStorage', () => {
    loadRecommendations();
    expect(document.getElementById('recommendationsContainer').innerHTML)
      .toContain('No recommendations found');
  });

  test('renders list of movie titles if valid data exists', () => {
    sessionStorage.setItem('recommendations', JSON.stringify([
      { title: 'Divergent', year: 2014 },
      { title: 'The Maze Runner', year: 2014 },
    ]));
    sessionStorage.setItem('originalTitle', 'The Hunger Games');

    loadRecommendations();

    const container = document.getElementById('recommendationsContainer');
    expect(container).toHaveTextContent('Movies similar to "The Hunger Games":');
    expect(container).toHaveTextContent('Divergent (2014)');
    expect(container).toHaveTextContent('The Maze Runner (2014)');
  });

  test('shows error if JSON parsing fails', () => {
    sessionStorage.setItem('recommendations', 'invalid_json');
    console.error = jest.fn();

    loadRecommendations();

    expect(document.getElementById('recommendationsContainer').innerHTML)
      .toContain('Error displaying recommendations');
  });
});

describe('getMoreLikeThis', () => {
  test('stores data in sessionStorage and redirects on success', async () => {
    fetch.mockResponseOnce(JSON.stringify([
      { title: 'The Maze Runner', year: 2014 }
    ]));

    delete window.location;
    window.location = { href: '' };

    await getMoreLikeThis('The Hunger Games');

    expect(sessionStorage.getItem('recommendationsData')).toBeTruthy();
    expect(window.location.href).toBe('/Home/Recommendations');
  });

  test('shows alert and logs error on fetch failure', async () => {
    fetch.mockRejectOnce(() => Promise.reject("API error"));
    window.alert = jest.fn();
    console.error = jest.fn();

    await getMoreLikeThis('Bad Movie');

    expect(window.alert).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalled();
  });
});

describe('setupMoreLikeThisButtons', () => {
  test('fetches recommendations when "More Like This" button is clicked', async () => {
    fetch.mockResponseOnce(JSON.stringify([
      { title: 'Divergent', year: 2014 }
    ]));

    setupMoreLikeThisButtons();

    // simluates the clicking
    const button = document.querySelector('.btn-outline-secondary');
    fireEvent.click(button);

    // wait for async ipdates 
    await new Promise(resolve => setTimeout(resolve, 0));

    expect(fetch).toHaveBeenCalledWith('/Home/GetSimilarMovies?title=The%20Hunger%20Games');
    expect(sessionStorage.getItem('recommendations')).toBeTruthy();
    expect(sessionStorage.getItem('originalTitle')).toBe('The Hunger Games');
  });

  test('shows alert on rate limit error', async () => {
    fetch.mockResponseOnce(JSON.stringify({ message: "rate_limit_exceeded" }), { status: 429 });
    window.alert = jest.fn();

    setupMoreLikeThisButtons();
    fireEvent.click(document.querySelector('.btn-outline-secondary'));

    await new Promise(resolve => setTimeout(resolve, 0));

    expect(window.alert).toHaveBeenCalledWith(expect.stringContaining('⚠️'));
  });
});

describe('loadRecommendations - UI rendering and interaction', () => {
  beforeEach(() => {
    document.body.innerHTML = `
      <div id="recommendationsContainer"></div>
      <div id="modalTitle"></div>
      <img id="modalPoster" />
    `;

    sessionStorage.setItem('recommendations', JSON.stringify([
      {
        title: 'Interstellar',
        year: 2014,
        genres: ['Sci-Fi', 'Adventure'],
        overview: 'A team of explorers travel through a wormhole in space.',
        rating: 'PG-13',
        posterUrl: 'https://example.com/interstellar.jpg',
        services: ['Netflix', 'HBO Max']
      }
    ]));
    sessionStorage.setItem('originalTitle', 'Inception');
  });

  test('renders a movie card with title, year, and buttons', () => {
    loadRecommendations();

    const container = document.getElementById('recommendationsContainer');
    expect(container).toHaveTextContent('Interstellar (2014)');
    expect(container.querySelector('.btn-primary')).toBeInTheDocument();
    expect(container.querySelector('.btn-outline-secondary')).toBeInTheDocument();
  }); 

  test('clicking "More Like This" on recommendation triggers new fetch', async () => {
    // Add dummy spinner
    document.body.innerHTML += `<div id="loadingSpinner" style="display: none;"></div>`;
  
    fetch.mockResponseOnce(JSON.stringify([
      { title: 'The Martian', year: 2015 }
    ]));
  
    loadRecommendations();
    setupMoreLikeThisButtons();
  
    const moreLikeThisBtn = document.querySelector('.btn-outline-secondary');
  
    // Simulate card data needed by the button
    moreLikeThisBtn.closest('.movie-card').setAttribute('data-title', 'Interstellar');
  
    fireEvent.click(moreLikeThisBtn);
  
    // Allow promise to resolve
    await new Promise(resolve => setTimeout(resolve, 0));
  
    expect(fetch).toHaveBeenCalled();
    expect(sessionStorage.getItem('originalTitle')).not.toBeNull();
    expect(sessionStorage.getItem('recommendations')).toBeTruthy();
  });
  
});
