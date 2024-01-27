import React, { useState, useEffect } from 'react';

function WeatherComponent() {
  const [weatherData, setWeatherData] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchWeatherData = async () => {
    setIsLoading(true);
    setError(null);

    try {
      const response = await fetch('http://localhost:5293/weatherforecast');
      const data = await response.json();
      setWeatherData(data);
    } catch (error) {
      setError(error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div>
      <button onClick={fetchWeatherData} disabled={isLoading}>
        {isLoading ? 'Loading...' : 'Get Weather Forecast'}
      </button>
      {error && <p>Error fetching data: {error.message}</p>}
      {weatherData.length > 0 && (
        <ul>
          {weatherData.map((day, index) => (
            <li key={index}>
              <h4>
                {day.date} ({day.temperatureC}°C / {day.temperatureF}°F)
              </h4>
              <p>{day.summary}</p>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

export default WeatherComponent;
