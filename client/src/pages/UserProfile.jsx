import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import Header from '../components/Header';
import styles from './UserProfile.module.css';
import { getUserBookings, cancelBooking } from '../services/bookingService';
import { getUserProfile, updateUserProfile } from '../services/userService';


export default function UserProfile() {
  const { userId, role } = useAuth();
  const [activeTab, setActiveTab] = useState('profile');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [editMode, setEditMode] = useState(false);

  const [login, setLogin] = useState('');
  const [email, setEmail] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');


  const [bookings, setBookings] = useState([]);
  const [loadingBookings, setLoadingBookings] = useState(false);
  const [errorBookings, setErrorBookings] = useState('');

  useEffect(() => {
    if (activeTab === 'bookings' || activeTab === 'tickets') {
      fetchBookings();
    }
    // eslint-disable-next-line
  }, [activeTab]);

  const fetchBookings = async () => {
    setLoadingBookings(true);
    setErrorBookings('');
    try {
      const token = localStorage.getItem('token');
      const data = await getUserBookings(userId, token);
      setBookings(data);
    } catch (e) {
      setErrorBookings(e.message);
    } finally {
      setLoadingBookings(false);
    }
  };

  const handleCancel = async (id) => {
    if (!window.confirm('Ви дійсно хочете скасувати це бронювання?')) return;
    try {
      const token = localStorage.getItem('token');
      await cancelBooking(id, token);
      setBookings(bookings => bookings.map(b => b.id === id ? { ...b, status: 'Canceled' } : b));
    } catch (e) {
      alert(e.message);
    }
  };

  const handleSave = async () => {
    try {
      const token = localStorage.getItem('token');
      await updateUserProfile(userId, {
        name: firstName,
        surname: lastName,
      }, token);
      alert('Дані оновлено');
      setEditMode(false);
    } catch (e) {
      alert(e.message);
    }
  };

  useEffect(() => {
    if (activeTab === 'profile') {
      fetchUserProfile();
    }
  }, [activeTab]);

  const fetchUserProfile = async () => {
    try {
      const token = localStorage.getItem('token');
      const data = await getUserProfile(userId, token);
      setFirstName(data.firstName);
      setLastName(data.lastName);
      setEmail(data.email);
      setPhoneNumber(data.phoneNumber);
      setLogin(data.login);
    } catch (e) {
      alert(e.message);
    }
  };


  // Розділяємо бронювання
  const tickets = bookings.filter(b => b.status === 'completed');
  const activeBookings = bookings.filter(b => b.status === 'active');
  const canceledBookings = bookings.filter(b => b.status === 'cancelled');

  return (
    <>
      <Header />
      <div className={styles.container}>
        <nav className={styles.tabs}>
          <button
            className={activeTab === 'profile' ? styles.active : ''}
            onClick={() => setActiveTab('profile')}
          >
            Мій профіль
          </button>
          <button
            className={activeTab === 'tickets' ? styles.active : ''}
            onClick={() => setActiveTab('tickets')}
          >
            Мої квитки
          </button>
          <button
            className={activeTab === 'bookings' ? styles.active : ''}
            onClick={() => setActiveTab('bookings')}
          >
            Мої бронювання
          </button>
          <button onClick={() => window.location.href = '/'}>Пошук маршрутів</button>
        </nav>

        <div className={styles.content}>
          {activeTab === 'profile' && (
            <div className={styles.profile}>
            <h3>Особисті дані</h3>
            <p><strong>ID:</strong> {userId}</p>
            <p><strong>Роль:</strong> {role}</p>
            <p><strong>Логін:</strong> {login}</p>
            <p><strong>Email:</strong> {email}</p>
            <p><strong>Телефон:</strong> {phoneNumber}</p>

            <div className={styles.field}>
              <label>Ім’я:</label>
              {editMode ? (
                <input value={firstName} onChange={(e) => setFirstName(e.target.value)} />
              ) : (
                <span>{firstName || '(не задано)'}</span>
              )}
            </div>

            <div className={styles.field}>
              <label>Прізвище:</label>
              {editMode ? (
                <input value={lastName} onChange={(e) => setLastName(e.target.value)} />
              ) : (
                <span>{lastName || '(не задано)'}</span>
              )}
            </div>

            {editMode ? (
              <button className={styles.saveBtn} onClick={handleSave}>Зберегти</button>
            ) : (
              <button className={styles.editBtn} onClick={() => setEditMode(true)}>Редагувати</button>
            )}
        </div>
          )}

          {activeTab === 'tickets' && (
            <div>
              <h3>Мої квитки</h3>
              {loadingBookings && <p>Завантаження...</p>}
              {errorBookings && <p style={{ color: 'red' }}>{errorBookings}</p>}
              {!loadingBookings && tickets.length === 0 && <p>Квитків не знайдено.</p>}
              <ul className={styles.bookingList}>
                {tickets.map(b => (
                  <li key={b.id} className={styles.bookingCard}>
                    <div>
                      <div><strong>{b.fromStationName}</strong> → <strong>{b.toStationName}</strong></div>
                      <div>Відправлення: {new Date(b.departureDateTime).toLocaleString()}</div>
                      <div>Прибуття: {new Date(b.arrivalDateTime).toLocaleString()}</div>
                      <div>Місце: {b.seatCode}</div>
                      <div>Статус: <span className={styles.completed}>Квиток</span></div>
                      <div>Ціна: {b.price} грн</div>
                      <div>Час бронювання: {new Date(b.bookingTime).toLocaleString()}</div>
                    </div>
                  </li>
                ))}
              </ul>
            </div>
          )}

          {activeTab === 'bookings' && (
            <div>
              <h3>Мої бронювання</h3>
              {loadingBookings && <p>Завантаження...</p>}
              {errorBookings && <p style={{ color: 'red' }}>{errorBookings}</p>}
              {!loadingBookings && bookings.length === 0 && <p>Бронювань не знайдено.</p>}
              <ul className={styles.bookingList}>
                {/* Активні бронювання */}
                {activeBookings.map(b => (
                  <li key={b.id} className={styles.bookingCard}>
                    <div>
                      <div><strong>{b.fromStationName}</strong> → <strong>{b.toStationName}</strong></div>
                      <div>Відправлення: {new Date(b.departureDateTime).toLocaleString()}</div>
                      <div>Прибуття: {new Date(b.arrivalDateTime).toLocaleString()}</div>
                      <div>Місце: {b.seatCode}</div>
                      <div>Статус: {b.status}</div>
                      <div>Ціна: {b.price} грн</div>
                      <div>Час бронювання: {new Date(b.bookingTime).toLocaleString()}</div>
                    </div>
                    <button className={styles.cancelBtn} onClick={() => handleCancel(b.id)}>
                      Відмінити
                    </button>
                  </li>
                ))}

                {/* Скасовані бронювання */}
                {canceledBookings.map(b => (
                  <li key={b.id} className={`${styles.bookingCard} ${styles.canceled}`}>
                    <div>
                      <div><strong>{b.fromStationName}</strong> → <strong>{b.toStationName}</strong></div>
                      <div>Відправлення: {new Date(b.departureDateTime).toLocaleString()}</div>
                      <div>Прибуття: {new Date(b.arrivalDateTime).toLocaleString()}</div>
                      <div>Місце: {b.seatCode}</div>
                      <div>Статус: <span className={styles.canceledText}>Скасовано</span></div>
                      <div>Ціна: {b.price} грн</div>
                      <div>Час бронювання: {new Date(b.bookingTime).toLocaleString()}</div>
                    </div>
                  </li>
                ))}
              </ul>
            </div>
          )}

        </div>
      </div>
    </>
  );
}
