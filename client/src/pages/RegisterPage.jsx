import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './Auth.module.css';

function RegisterPage() {
  const [formData, setFormData] = useState({
    login: '',
    email: '',
    password: '',
    name: '',
    surname: '',
    phoneNumber: ''
  });

  const [error, setError] = useState('');
  const navigate = useNavigate();

  const validateEmail = (email) =>
    /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);

  const validatePhone = (phone) =>
    /^\+?\d{10,15}$/.test(phone); // допускається +380... або 0...

  const handleChange = (e) => {
    setFormData(prev => ({
      ...prev,
      [e.target.name]: e.target.value
    }));
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    setError('');

    // Валідація email і телефону
    if (!validateEmail(formData.email)) {
      setError('Некоректний email');
      return;
    }

    if (!validatePhone(formData.phoneNumber)) {
      setError('Телефон має містити лише цифри (від 10 до 15 знаків), можна з + на початку');
      return;
    }

    try {
      const res = await fetch('http://localhost:5086/api/Users/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData)
      });

      if (!res.ok) {
        const errorData = await res.json();
        throw new Error(errorData.message || 'Помилка реєстрації');
      }

      const data = await res.json();
      alert(data.message); // "Користувача успішно зареєстровано."
      navigate('/login');
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className={styles.authContainer}>
      <h2>Реєстрація</h2>
      <form onSubmit={handleRegister} className={styles.form}>
        <input
          type="text"
          name="login"
          placeholder="Логін"
          value={formData.login}
          onChange={handleChange}
          required
        />
        <input
          type="email"
          name="email"
          placeholder="Email"
          value={formData.email}
          onChange={handleChange}
          required
        />
        <input
          type="password"
          name="password"
          placeholder="Пароль"
          value={formData.password}
          onChange={handleChange}
          required
        />
        <input
          type="text"
          name="name"
          placeholder="Ім’я"
          value={formData.name}
          onChange={handleChange}
          required
        />
        <input
          type="text"
          name="surname"
          placeholder="Прізвище"
          value={formData.surname}
          onChange={handleChange}
          required
        />
        <input
          type="tel"
          name="phoneNumber"
          placeholder="Телефон"
          value={formData.phoneNumber}
          onChange={handleChange}
          required
        />

        {error && <div className={styles.error}>{error}</div>}

        <button type="submit">Зареєструватися</button>
      </form>
      <div className={styles.link} onClick={() => navigate('/login')}>
        Вже маєте акаунт? Увійти
      </div>
    </div>
  );
}

export default RegisterPage;
