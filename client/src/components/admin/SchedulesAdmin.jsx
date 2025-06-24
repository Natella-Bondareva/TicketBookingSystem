import React, { useState, useEffect } from 'react';

export default function SchedulesAdmin() {
  const [schedules, setSchedules] = useState([]);
  const [routes, setRoutes] = useState([]);
  const [stops, setStops] = useState([]); // [{stationId, stationName, arrivalDateTime, departureDateTime}]
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [editSchedule, setEditSchedule] = useState(null);
  const [form, setForm] = useState({
    routeId: '',
    date: ''
  });

  useEffect(() => {
    fetchSchedules();
    fetchRoutes();
  }, []);

  const fetchSchedules = async () => {
    setLoading(true);
    setError('');
    try {
      const res = await fetch('http://localhost:5086/api/Schedules');
      if (!res.ok) throw new Error('Не вдалося отримати розклади');
      const data = await res.json();
      setSchedules(data);
    } catch (e) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  };

  const fetchRoutes = async () => {
    try {
      const res = await fetch('http://localhost:5086/api/Routes');
      if (!res.ok) throw new Error('Не вдалося отримати маршрути');
      const data = await res.json();
      setRoutes(data);
    } catch (e) {
      setError(e.message);
    }
  };

  const handleChange = e => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleRouteChange = e => {
    const routeId = e.target.value;
    setForm({ ...form, routeId });
    const route = routes.find(r => r.id === Number(routeId));
    if (route) {
      setStops(
        route.stops.map(stop => ({
          stationId: stop.stationId,
          stationName: stop.stationName,
          arrivalDateTime: '',
          departureDateTime: ''
        }))
      );
    } else {
      setStops([]);
    }
  };

  const handleStopDateTimeChange = (idx, field, value) => {
    setStops(stops =>
      stops.map((s, i) =>
        i === idx ? { ...s, [field]: value } : s
      )
    );
  };

  const handleEdit = schedule => {
    setEditSchedule(schedule);
    setForm({
      routeId: schedule.routeId,
      date: schedule.date?.slice(0, 10) || ''
    });
    setStops(
      Array.isArray(schedule.stops)
        ? schedule.stops.map(stop => ({
            stationId: stop.stationId,
            stationName: stop.stationName,
            arrivalDateTime: stop.arrivalDateTime ? stop.arrivalDateTime.slice(0, 16) : '',
            departureDateTime: stop.departureDateTime ? stop.departureDateTime.slice(0, 16) : ''
          }))
        : []
    );
  };

  const handleDelete = async id => {
    if (!window.confirm('Видалити розклад?')) return;
    try {
      const res = await fetch(`http://localhost:5086/api/Schedules/${id}`, {
        method: 'DELETE',
      });
      if (!res.ok) throw new Error('Не вдалося видалити розклад');
      setSchedules(schedules.filter(s => s.id !== id));
    } catch (e) {
      alert(e.message);
    }
  };

  const handleSubmit = async e => {
    e.preventDefault();
    try {
      let res;
      const payload = {
        routeId: Number(form.routeId),
        date: form.date,
        stops: stops.map(s => ({
          stationId: s.stationId,
          arrivalDateTime: s.arrivalDateTime,
          departureDateTime: s.departureDateTime
        }))
      };
      // Логування payload
      console.log('SCHEDULE PAYLOAD:', JSON.stringify(payload, null, 2));
      if (editSchedule) {
        res = await fetch(`http://localhost:5086/api/Schedules/${editSchedule.id}`, {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload),
        });
      } else {
        res = await fetch('http://localhost:5086/api/Schedules', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload),
        });
      }
      if (!res.ok) throw new Error('Не вдалося зберегти розклад');
      setForm({ routeId: '', date: '' });
      setStops([]);
      setEditSchedule(null);
      fetchSchedules();
    } catch (e) {
      alert(e.message);
    }
  };

  const handleCancel = () => {
    setEditSchedule(null);
    setForm({ routeId: '', date: '' });
    setStops([]);
  };

  return (
    <div>
      <h2>Розклади</h2>
      {loading && <p>Завантаження...</p>}
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <table style={{ width: '100%', borderCollapse: 'collapse', marginBottom: 24 }}>
        <thead>
          <tr>
            <th>ID</th>
            <th>Маршрут</th>
            <th>Дата</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {schedules.map(schedule => (
            <tr key={schedule.id}>
              <td>{schedule.id}</td>
              <td>
                {routes.find(r => r.id === schedule.routeId)
                  ? `${routes.find(r => r.id === schedule.routeId).stops?.[0]?.stationName} → ${routes.find(r => r.id === schedule.routeId).stops?.[routes.find(r => r.id === schedule.routeId).stops.length - 1]?.stationName}`
                  : schedule.routeId}
              </td>
              <td>{schedule.date?.slice(0, 10)}</td>
              <td>
                <button onClick={() => handleEdit(schedule)}>Редагувати</button>
                <button onClick={() => handleDelete(schedule.id)}>Видалити</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <h3>{editSchedule ? 'Редагувати розклад' : 'Додати розклад'}</h3>
      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: 16, marginTop: 12 }}>
        <select
          name="routeId"
          value={form.routeId}
          onChange={handleRouteChange}
          required
        >
          <option value="">Оберіть маршрут</option>
          {routes.map(r => (
            <option key={r.id} value={r.id}>
              {r.stops?.[0]?.stationName} → {r.stops?.[r.stops.length - 1]?.stationName}
            </option>
          ))}
        </select>
        <input
          type="date"
          name="date"
          value={form.date}
          onChange={handleChange}
          required
        />
        {stops.length > 0 && (
          <div style={{ border: '1px solid #e0e0e0', borderRadius: 6, padding: 12 }}>
            <b>Зупинки маршруту:</b>
            {stops.map((stop, idx) => (
              <div key={stop.stationId} style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 8 }}>
                <span>{stop.stationName}</span>
                <input
                  type="datetime-local"
                  value={stop.arrivalDateTime}
                  onChange={e => handleStopDateTimeChange(idx, 'arrivalDateTime', e.target.value)}
                  required
                  placeholder="Дата і час прибуття"
                  style={{ minWidth: 180 }}
                />
                <input
                  type="datetime-local"
                  value={stop.departureDateTime}
                  onChange={e => handleStopDateTimeChange(idx, 'departureDateTime', e.target.value)}
                  required
                  placeholder="Дата і час відправлення"
                  style={{ minWidth: 180 }}
                />
              </div>
            ))}
          </div>
        )}
        <button type="submit">{editSchedule ? 'Зберегти' : 'Додати'}</button>
        {editSchedule && <button type="button" onClick={handleCancel}>Скасувати</button>}
      </form>
    </div>
  );
}