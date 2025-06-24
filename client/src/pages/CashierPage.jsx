import React, { useState } from 'react';
import Header from '../components/Header';
import styles from './CashierPage.module.css';
import { useAuth } from '../context/AuthContext';
import Profile from '../pages/Profile';
import { useNavigate } from 'react-router-dom';

//


// Сервіси для пошуку та оплати (реалізуйте відповідно до вашого API)
import { searchBookingsByClientOrBookingId, payBooking } from '../services/cashierService';

function CashierPage() {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('pay');
  const [searchClientId, setSearchClientId] = useState('');
  const [searchBookingId, setSearchBookingId] = useState('');
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [showPaymentModal, setShowPaymentModal] = useState(false);
  const [selectedBooking, setSelectedBooking] = useState(null);
  const [amount, setAmount] = useState('');
  const [paymentMethodId, setPaymentMethodId] = useState('1'); // за замовчуванням "cash"

  const { token, userId, isAuthenticated } = useAuth(); // userId — це cashierId
  

  const handleSearch = async () => {
    const jwt = token || localStorage.getItem('token');
    setError('');
    setLoading(true);
    try {
      let data = await searchBookingsByClientOrBookingId(searchClientId, searchBookingId, jwt);

      // Якщо пошук по bookingId — це один об'єкт, інакше може бути масив або об'єкт
      if (searchBookingId) {
        data = data ? [data] : [];
      } else if (data && !Array.isArray(data)) {
        data = [data];
      }
      setBookings(data || []);
    } catch (e) {
      setError(e.message || 'Помилка пошуку');
    } finally {
      setLoading(false);
    }
  };

  const openPaymentModal = (booking) => {
    setSelectedBooking(booking);
    setAmount(booking.price || '');
    setPaymentMethodId('1'); // за замовчуванням "cash"
    setShowPaymentModal(true);
  };

  const handlePayment = async () => {
    try {
      await payBooking(
        {
          bookingId: selectedBooking.id,
          cashierId: userId,
          amount: Number(amount),
          paymentMethodId: Number(paymentMethodId)
        },
        token || localStorage.getItem('token')
      );
      alert('Оплату проведено успішно!');
      setShowPaymentModal(false);
      setBookings(bookings =>
        bookings.map(b => b.id === selectedBooking.id ? { ...b, status: 'completed' } : b)
      );
    } catch (e) {
      alert(e.message || 'Помилка оплати');
    }
  };

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
            className={activeTab === 'pay' ? styles.active : ''}
            onClick={() => setActiveTab('pay')}
          >
            Оплата
          </button>
          <button
            className={activeTab === 'bookings' ? styles.active : ''}
            onClick={() => navigate("/")}
          >
            Бронювання
          </button>
        </nav>

        {activeTab === 'profile' && (
            <Profile />
        )}

        {activeTab === 'pay' && (
          <div className={styles.paySection}>
            <h2>Пошук бронювання для оплати</h2>
            <div className={styles.searchFields}>
              <input
                type="text"
                placeholder="ID клієнта"
                value={searchClientId}
                onChange={e => setSearchClientId(e.target.value)}
              />
              <input
                type="text"
                placeholder="ID бронювання"
                value={searchBookingId}
                onChange={e => setSearchBookingId(e.target.value)}
              />
              <button onClick={handleSearch}>Пошук</button>
              
            </div>
            {loading && <p>Завантаження...</p>}
            {error && <p className={styles.error}>{error}</p>}
            
            <ul className={styles.bookingList}>
              {searchBookingId
                ? // Пошук по номеру бронювання
                  bookings.map(b => (
                    <li key={b.id} className={styles.bookingCard}>
                      <div>
                        <div><strong>{b.fromStationName}</strong> → <strong>{b.toStationName}</strong></div>
                        <div>Відправлення: {new Date(b.departureDateTime).toLocaleString()}</div>
                        <div>Місце: {b.seatCode}</div>
                        <div>Статус: {b.status}</div>
                        <div>Ціна: {b.price} грн</div>
                      </div>
                      {b.status === 'completed' && (
                        <span className={styles.paidLabel}>Бронювання успішно сплачене</span>
                      )}
                      {b.status === 'cancelled' && (
                        <span className={styles.canceledLabel}>Бронювання скасовано</span>
                      )}
                      {b.status !== 'completed' && b.status !== 'cancelled' && (
                        <button className={styles.payBtn} onClick={() => openPaymentModal(b)}>
                          Оплатити
                        </button>
                      )}
                    </li>
                  ))
                : // Пошук по користувачу
                  [
                    // Активні бронювання (не Paid, не Canceled)
                    ...bookings
                      .filter(b => b.status !== 'completed' && b.status !== 'cancelled')
                      .map(b => (
                        <li key={b.id} className={styles.bookingCard}>
                          <div>
                            <div><strong>{b.fromStationName}</strong> → <strong>{b.toStationName}</strong></div>
                            <div>Відправлення: {new Date(b.departureDateTime).toLocaleString()}</div>
                            <div>Місце: {b.seatCode}</div>
                            <div>Статус: {b.status}</div>
                            <div>Ціна: {b.price} грн</div>
                          </div>
                          <button className={styles.payBtn} onClick={() => openPaymentModal(b)}>
                            Оплатити
                          </button>
                        </li>
                      )),
                    // Скасовані бронювання (в кінці, без кнопки)
                    ...bookings
                      .filter(b => b.status === 'cancelled')
                      .map(b => (
                        <li key={b.id} className={`${styles.bookingCard} ${styles.canceledCard}`}>
                          <div>
                            <div><strong>{b.fromStationName}</strong> → <strong>{b.toStationName}</strong></div>
                            <div>Відправлення: {new Date(b.departureDateTime).toLocaleString()}</div>
                            <div>Місце: {b.seatCode}</div>
                            <div>Статус: {b.status}</div>
                            <div>Ціна: {b.price} грн</div>
                          </div>
                          <span className={styles.canceledLabel}>Бронювання скасовано</span>
                        </li>
                      )),
                  ]
              }
            </ul>
          </div>
        )}

        {activeTab === 'bookings' && (
          <div>
            <h2>Всі бронювання (реалізуйте потрібний функціонал)</h2>
            {/* Тут можна додати таблицю всіх бронювань, якщо потрібно */}
          </div>
        )}

        {showPaymentModal && (
          <div className={styles.modalOverlay}>
            <div className={styles.modal}>
              <h3>Оплата бронювання #{selectedBooking.id}</h3>
              <div>
                <label>Сума:</label>
                <input
                  type="number"
                  value={amount}
                  onChange={e => setAmount(e.target.value)}
                  min="1"
                />
              </div>
              <div>
                <label>Спосіб оплати:</label>
                <select
                  value={paymentMethodId}
                  onChange={e => setPaymentMethodId(e.target.value)}
                >
                  <option value="1">Готівка</option>
                  <option value="2">Картка</option>
                </select>
              </div>
              <div className={styles.modalActions}>
                <button onClick={handlePayment}>Підтвердити оплату</button>
                <button onClick={() => setShowPaymentModal(false)}>Скасувати</button>
              </div>
            </div>
          </div>
        )}
      </div>
    </>
  );
}

export default CashierPage;