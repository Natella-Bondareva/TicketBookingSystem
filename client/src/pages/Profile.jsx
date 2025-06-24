import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import Header from '../components/Header';
import styles from './UserProfile.module.css';
import { getUserProfile, updateUserProfile } from '../services/userService';

export default function Profile() {
  const { userId, role } = useAuth();
  const [activeTab, setActiveTab] = useState('profile');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [editMode, setEditMode] = useState(false);

  const [login, setLogin] = useState('');
  const [email, setEmail] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');

  // Завантаження профілю при активації вкладки
  useEffect(() => {
    if (activeTab === 'profile') {
      fetchUserProfile();
    }
    // eslint-disable-next-line
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

  const handleSave = async () => {
    try {
      const token = localStorage.getItem('token');
      await updateUserProfile(userId, {
        name: firstName,
        surname: lastName,
      }, token);
      alert('Дані оновлено');
      setEditMode(false);
      fetchUserProfile(); // Оновити дані після збереження
    } catch (e) {
      alert(e.message);
    }
  };

  return (
    <>
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
      </div>
    </>
  );
}
