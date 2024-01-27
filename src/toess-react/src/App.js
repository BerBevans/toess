import logo from './logo.svg';
import './App.css';
import WeatherComponent  from './components/weather-forecast.js';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <h1>Test</h1>
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
      <main>
      <WeatherComponent />
      </main>
      <footer>
        <p>Footer</p>
      </footer>
    </div>
  );
}

export default App;
