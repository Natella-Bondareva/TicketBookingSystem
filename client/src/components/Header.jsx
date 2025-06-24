import React from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import styles from './Header.module.css';

function Header() {
  const { isAuthenticated, role, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <header className={styles.header}>
      <h2 className={styles.logo} onClick={() => navigate('/')}>TicketBooking</h2>
      <div>
        {isAuthenticated && role === 'cashier' && (
          <>
            <button className={styles.btn} onClick={() => navigate('/cashier')}>Панель Касира</button>
            <button className={styles.btn} onClick={handleLogout}>Вийти</button>
          </>
        )}
        {isAuthenticated && role === 'client' && (
          <>
            <Link to="/profile" className={styles.btn}>Мій профіль</Link>
            <button className={styles.btn} onClick={handleLogout}>Вийти</button>
          </>
        )}
        {isAuthenticated && role === 'admin' && (
          <>
            <Link to="/admin" className={styles.btn}>Адмін-панель</Link>
            <button className={styles.btn} onClick={handleLogout}>Вийти</button>
          </>
        )}
        {!isAuthenticated && (
          <>
            <button className={styles.btn} onClick={() => navigate('/login')}>Вхід</button>
            <button className={styles.btn} onClick={() => navigate('/register')}>Реєстрація</button>
          </>
        )}
      </div>
    </header>
  );
}

export default Header;