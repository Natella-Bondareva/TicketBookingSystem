import React, { useState, useEffect } from 'react';
import styles from './StationsAdmin.module.css';

export default function StationsAdmin() {
  const [stations, setStations] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [editStation, setEditStation] = useState(null);
  const [form, setForm] = useState({ name: '', city: '', latitude: '', longitude: '' });
  const [search, setSearch] = useState('');

  // Завантаження станцій
  useEffect(() => {
    // Завжди завантажувати всі станції при відкритті вкладки
    fetchStations();
  }, []);

  const fetchStations = async (searchName = '') => {
    setLoading(true);
    setError('');
    try {
      let url = 'http://localhost:5086/api/Stations';
      if (searchName) {
        url = `http://localhost:5086/api/Stations/search?name=${encodeURIComponent(searchName)}`;
      }
      const res = await fetch(url);
      if (!res.ok) throw new Error('Не вдалося отримати станції');
      const data = await res.json();
      setStations(data);
    } catch (e) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = e => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleEdit = station => {
    setEditStation(station);
    setForm(station);
  };

  const handleDelete = async id => {
    if (!window.confirm('Видалити станцію?')) return;
    try {
      const res = await fetch(`http://localhost:5086/api/Stations/${id}`, {
        method: 'DELETE',
      });
      if (!res.ok) throw new Error('Не вдалося видалити станцію');
      setStations(stations.filter(s => s.id !== id));
    } catch (e) {
      alert(e.message);
    }
  };

  const handleSubmit = async e => {
    e.preventDefault();
    try {
      let res;
      if (editStation) {
        res = await fetch(`http://localhost:5086/api/Stations/${editStation.id}`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(form),
        });
      } else {
        res = await fetch('http://localhost:5086/api/Stations', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(form),
        });
      }
      if (!res.ok) throw new Error('Не вдалося зберегти станцію');
      setForm({ name: '', city: '', latitude: '', longitude: '' });
      setEditStation(null);
      fetchStations(search); // оновити список з урахуванням пошуку
    } catch (e) {
      alert(e.message);
    }
  };

  const handleCancel = () => {
    setEditStation(null);
    setForm({ name: '', city: '', latitude: '', longitude: '' });
  };

  const handleSearch = (e) => {
    e.preventDefault();
    fetchStations(search.trim());
  };

  const handleReset = () => {
    setSearch('');
    fetchStations();
  };

  return (
    <div>
      <h2>Станції</h2>
      <form onSubmit={handleSearch} style={{ marginBottom: 16 }}>
        <input
          type="text"
          placeholder="Пошук за назвою"
          value={search}
          onChange={e => setSearch(e.target.value)}
          style={{ padding: '8px 10px', borderRadius: 5, border: '1px solid #b0bec5', marginRight: 8 }}
        />
        <button type="submit" className={styles.formRow}>Пошук</button>
        <button type="button" className={styles.formRow} onClick={handleReset}>
          Скинути
        </button>
      </form>
      {loading && <p>Завантаження...</p>}
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <table className={styles.table}>
        <thead>
          <tr>
            <th>Назва</th>
            <th>Місто</th>
            <th>Широта</th>
            <th>Довгота</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {stations.map(station => (
            <tr key={station.id}>
              <td>{station.name}</td>
              <td>{station.city}</td>
              <td>{station.latitude}</td>
              <td>{station.longitude}</td>
              <td>
                <button onClick={() => handleEdit(station)}>Редагувати</button>
                <button onClick={() => handleDelete(station.id)}>Видалити</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <h3>{editStation ? 'Редагувати станцію' : 'Додати станцію'}</h3>
      <form onSubmit={handleSubmit} className={styles.formRow}>
        <input
          name="name"
          placeholder="Назва"
          value={form.name}
          onChange={handleChange}
          required
        />
        <input
          name="city"
          placeholder="Місто"
          value={form.city}
          onChange={handleChange}
          required
        />
        <input
          name="latitude"
          placeholder="Широта"
          value={form.latitude}
          onChange={handleChange}
          required
        />
        <input
          name="longitude"
          placeholder="Довгота"
          value={form.longitude}
          onChange={handleChange}
          required
        />
        <button type="submit">{editStation ? 'Зберегти' : 'Додати'}</button>
        {editStation && <button type="button" onClick={handleCancel}>Скасувати</button>}
      </form>
    </div>
  );
}