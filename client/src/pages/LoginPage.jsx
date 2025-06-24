import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './Auth.module.css';
import { useAuth } from '../context/AuthContext';

export default function LoginPage() {
  const [loginInput, setLoginInput] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');
    try {
      const res = await fetch('http://localhost:5086/api/Users/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ login: loginInput, password }),
      });

      if (!res.ok) throw new Error('Невірний логін або пароль');

      const data = await res.json();

      login({ token: data.token, role: data.role, userId: data.userId });

      switch (data.role) {
        case 'admin':
          navigate('/admin');
          break;
        case 'cashier':
          navigate('/cashier');
          break;
        default:
          navigate('/');
          break;
      }
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className={styles.authContainer}>
      <h2>Вхід</h2>
      <form onSubmit={handleLogin} className={styles.form}>
        <input
          type="text"
          placeholder="Логін"
          value={loginInput}
          onChange={e => setLoginInput(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Пароль"
          value={password}
          onChange={e => setPassword(e.target.value)}
          required
        />
        {error && <div className={styles.error}>{error}</div>}
        <button type="submit">Увійти</button>
      </form>
      <div className={styles.link} onClick={() => navigate('/register')}>
        Ще не маєте акаунту? Зареєструватися
      </div>
    </div>
  );
}
