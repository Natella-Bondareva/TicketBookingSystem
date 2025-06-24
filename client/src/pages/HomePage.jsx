import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { searchRoutesApi } from '../services/routeService';
import { bookRoute } from '../services/bookingService';
import Header from '../components/Header';
import { useAuth } from '../context/AuthContext'; // якщо є AuthContext
import styles from './HomePage.module.css';

function HomePage() {
  const [from, setFrom] = useState('');
  const [to, setTo] = useState('');
  const [date, setDate] = useState('');
  const [withTransfers, setWithTransfers] = useState(true);

  const { mutate, data, isPending, isError, error } = useMutation({
    mutationFn: searchRoutesApi
  });

  const { token, userId, isAuthenticated } = useAuth();

  const handleSearch = () => {
    if (!from || !to || !date) {
      alert('Усі поля обовʼязкові');
      return;
    }
    mutate({
      fromStation: from,
      toStation: to,
      date,
      allowTransfers: withTransfers 
    });
  };

    const handleBook = async (route) => {
    const uid = userId || Number(localStorage.getItem('userId'));
    const jwt = token || localStorage.getItem('token');

    if (!jwt || !uid) {
        alert('Для бронювання необхідно увійти в акаунт!');
        return;
    }

    try {
        if (withTransfers === false) {
        // Прямий маршрут
        await bookRoute({
            scheduleId: route.scheduleId,
            fromStationId: route.fromStationId,
            toStationId: route.toStationId,
            userId: Number(uid),
        }, jwt);
        alert('Бронювання успішне!');
        } else if (withTransfers === true) {

        // Маршрут з пересадкою — двічі bookRoute
        await bookRoute({
            scheduleId: route.firstScheduleId,
            fromStationId: route.firstFromStationId,
            toStationId: route.firstToStationId,
            userId: Number(uid),
        }, jwt);

        await bookRoute({
            scheduleId: route.secondScheduleId,
            fromStationId: route.secondFromStationId,
            toStationId: route.secondToStationId,
            userId: Number(uid),
        }, jwt);

        alert('Обидва бронювання успішні!');
        } else {
        alert('Неможливо визначити тип маршруту для бронювання');
        }
    } catch (e) {
        alert(e.message || 'Помилка бронювання');
    }
    };

  return (
    <>
      <Header />
      <div className={styles.container}>
        <h1 className={styles.title}>Пошук маршрутів</h1>
        <div className={styles.searchBox}>
          <div className={styles.field}>
            <label className={styles.label}>Звідки</label>
            <input
              className={styles.input}
              type="text"
              placeholder="Введіть місто"
              value={from}
              onChange={(e) => setFrom(e.target.value)}
            />
          </div>
          <div className={styles.field}>
            <label className={styles.label}>Куди</label>
            <input
              className={styles.input}
              type="text"
              placeholder="Введіть місто"
              value={to}
              onChange={(e) => setTo(e.target.value)}
            />
          </div>
          <div className={styles.field}>
            <label className={styles.label}>Дата</label>
            <input
              className={styles.input}
              type="date"
              value={date}
              onChange={(e) => setDate(e.target.value)}
            />
          </div>
          <div className={styles.fieldCheckbox}>
            <label className={styles.labelCheckbox}>
              <input
                className={styles.checkbox}
                type="checkbox"
                checked={withTransfers}
                onChange={(e) => setWithTransfers(e.target.checked)}
              />
              З пересадками
            </label>
          </div>
          <button className={styles.searchBtn} onClick={handleSearch}>
            Знайти
          </button>
        </div>

        {isPending && <p>Пошук...</p>}
        {isError && <p style={{ color: 'red' }}>Помилка: {error.message}</p>}

        {data && !withTransfers && (
        <div className={styles.routesList}>
            {data.map((route) => (
            <div key={route.scheduleId} className={styles.routeCard}>
                <div className={styles.routeLeft}>
                <h3>{route.routeName}</h3>
                <p>
                    {route.fromStation} {new Date(route.departureTime).toLocaleTimeString()} → {route.toStation} {new Date(route.arrivalTime).toLocaleTimeString()}
                </p>
                <p>
                    {route.seatsAvailable
                    ? route.freeSeats.length < 5
                        ? `Доступних місць: ${route.freeSeats.length}`
                        : 'Місця в наявності'
                    : 'Немає вільних місць'}
                </p>
                <button onClick={() => handleDetails(route)} className={styles.detailsBtn}>
                    Деталі маршруту
                </button>
                </div>
                <div className={styles.routeRight}>
                <div className={styles.price}>{route.price} грн</div>
                <button
                    className={styles.bookBtn}
                    disabled={!route.seatsAvailable}
                    onClick={() => handleBook(route)}
                >
                    Бронювати
                </button>
                </div>
            </div>
            ))}
        </div>
        )}
        {data && withTransfers && (
        <div className={styles.routesList}>
            {data.map((route, index) => (
            <div key={index} className={styles.routeCard}>
                <div className={styles.routeLeft}>
                <h3>{route.firstFromStation} → {route.secondToStation} (з пересадкою)</h3>
                <p>
                    {route.firstFromStation} → {route.firstToStation}: {new Date(route.firstDeparture).toLocaleTimeString()} – {new Date(route.firstArrival).toLocaleTimeString()}
                </p>
                <p>
                    🔄 Пересадка
                </p>
                <p>
                    {route.secondFromStation} → {route.secondToStation}: {new Date(route.secondDeparture).toLocaleTimeString()} – {new Date(route.secondArrival).toLocaleTimeString()}
                </p>
                <button onClick={() => handleDetails(route)} className={styles.detailsBtn}>
                    Деталі маршруту
                </button>
                </div>
                <div className={styles.routeRight}>
                <div className={styles.price}>{route.totalPrice} грн</div>
                <button className={styles.bookBtn} onClick={() => handleBook(route)}>
                    Бронювати
                </button>
                </div>
            </div>
            ))}
        </div>
        )}
        {data && data.length === 0 && <p>Маршрути не знайдено</p>}  
      </div>
    </>
  );
}

export default HomePage;
