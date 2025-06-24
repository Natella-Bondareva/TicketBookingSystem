import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';

export default function RoutesAdmin() {
  const { token } = useAuth();
  const [routes, setRoutes] = useState([]);
  const [stations, setStations] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [editRoute, setEditRoute] = useState(null);
  const [form, setForm] = useState({
    stops: []
  });

  // Завантаження маршрутів та станцій
  useEffect(() => {
    fetchRoutes();
    fetchStations();
    // eslint-disable-next-line
  }, []);

  const fetchRoutes = async () => {
    setLoading(true);
    setError('');
    try {
      const res = await fetch('http://localhost:5086/api/Routes', {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });
      if (!res.ok) throw new Error('Не вдалося отримати маршрути');
      const data = await res.json();
      setRoutes(data);
    } catch (e) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  };

  const fetchStations = async () => {
    try {
      const res = await fetch('http://localhost:5086/api/Stations', {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });
      if (!res.ok) throw new Error('Не вдалося отримати станції');
      const data = await res.json();
      setStations(data);
    } catch (e) {
      setError(e.message);
    }
  };

  // Додаємо станцію до маршруту
  const handleAddStop = () => {
    setForm({
      ...form,
      stops: [
        ...form.stops,
        {
          stationId: '',
          stationName: '',
          stopOrder: form.stops.length + 1
        }
      ]
    });
  };

  // Змінюємо станцію в маршруті
  const handleStopChange = (idx, value) => {
    const station = stations.find(s => s.id === Number(value));
    const updatedStops = form.stops.map((stop, i) =>
      i === idx
        ? {
            ...stop,
            stationId: Number(value),
            stationName: station ? station.name : '',
          }
        : stop
    );
    setForm({ ...form, stops: updatedStops });
  };

  // Видалити станцію з маршруту
  const handleRemoveStop = idx => {
    const updatedStops = form.stops.filter((_, i) => i !== idx)
      .map((stop, i) => ({ ...stop, stopOrder: i + 1 }));
    setForm({ ...form, stops: updatedStops });
  };

  // Перемістити станцію вгору/вниз
  const moveStop = (idx, direction) => {
    const newStops = [...form.stops];
    const targetIdx = idx + direction;
    if (targetIdx < 0 || targetIdx >= newStops.length) return;
    [newStops[idx], newStops[targetIdx]] = [newStops[targetIdx], newStops[idx]];
    // Оновити stopOrder
    setForm({
      ...form,
      stops: newStops.map((stop, i) => ({ ...stop, stopOrder: i + 1 }))
    });
  };

  const handleEdit = route => {
    setEditRoute(route);
    setForm({
      stops: route.stops.map(stop => ({
        stationId: stop.stationId,
        stationName: stop.stationName,
        stopOrder: stop.stopOrder
      }))
    });
  };

  const handleDelete = async id => {
    if (!window.confirm('Видалити маршрут?')) return;
    try {
      const res = await fetch(`http://localhost:5086/api/Routes/${id}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });
      if (!res.ok) throw new Error('Не вдалося видалити маршрут');
      setRoutes(routes.filter(r => r.id !== id));
    } catch (e) {
      alert(e.message);
    }
  };

  const handleSubmit = async e => {
    e.preventDefault();
    try {
      let res;
      if (form.stops.length < 2) {
        alert('Маршрут має містити мінімум дві станції');
        return;
      }
      const payload = {
        startStationId: Number(form.stops[0].stationId),
        endStationId: Number(form.stops[form.stops.length - 1].stationId),
        intermediateStationIds: form.stops
          .slice(1, -1)
          .map(stop => Number(stop.stationId))
      };
      if (editRoute) {
        res = await fetch(`http://localhost:5086/api/Routes/${editRoute.id}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
          },
          body: JSON.stringify(payload),
        });
      } else {
        res = await fetch('http://localhost:5086/api/Routes', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
          },
          body: JSON.stringify(payload),
        });
      }
      if (!res.ok) throw new Error('Не вдалося зберегти маршрут');
      setForm({ stops: [] });
      setEditRoute(null);
      fetchRoutes();
    } catch (e) {
      alert(e.message);
    }
  };

  const handleCancel = () => {
    setEditRoute(null);
    setForm({ stops: [] });
  };

  return (
    <div>
      <h2>Маршрути</h2>
      {loading && <p>Завантаження...</p>}
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <table style={{ width: '100%', borderCollapse: 'collapse', marginBottom: 24 }}>
        <thead>
          <tr>
            <th>ID</th>
            <th>Початкова станція</th>
            <th>Кінцева станція</th>
            <th>Проміжні станції (порядок)</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {routes.map(route => (
            <tr key={route.id}>
              <td>{route.id}</td>
              <td>{route.stops?.[0]?.stationName || ''}</td>
              <td>{route.stops?.[route.stops.length - 1]?.stationName || ''}</td>
              <td>
                {route.stops
                  .slice(1, -1)
                  .map(stop => `${stop.stationName} (${stop.stopOrder})`)
                  .join(', ')}
              </td>
              <td>
                <button onClick={() => handleEdit(route)}>Редагувати</button>
                <button onClick={() => handleDelete(route.id)}>Видалити</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <h3>{editRoute ? 'Редагувати маршрут' : 'Додати маршрут'}</h3>
      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: 12, marginTop: 12, maxWidth: 600 }}>
        <div>
          <b>Список зупинок (у порядку слідування):</b>
          {form.stops.map((stop, idx) => (
            <div key={idx} style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 4 }}>
              <span>{idx + 1}.</span>
              <select
                value={stop.stationId}
                onChange={e => handleStopChange(idx, e.target.value)}
                required
              >
                <option value="">Оберіть станцію</option>
                {stations.map(s => (
                  <option key={s.id} value={s.id}>{s.name} ({s.city})</option>
                ))}
              </select>
              <button type="button" onClick={() => moveStop(idx, -1)} disabled={idx === 0}>▲</button>
              <button type="button" onClick={() => moveStop(idx, 1)} disabled={idx === form.stops.length - 1}>▼</button>
              <button type="button" onClick={() => handleRemoveStop(idx)}>Видалити</button>
            </div>
          ))}
          <button type="button" onClick={handleAddStop}>Додати зупинку</button>
        </div>
        <button type="submit">{editRoute ? 'Зберегти' : 'Додати'}</button>
        {editRoute && <button type="button" onClick={handleCancel}>Скасувати</button>}
      </form>
    </div>
  );
}