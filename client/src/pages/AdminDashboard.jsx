import React, { useState } from 'react';
import Header from '../components/Header';
import RoutesAdmin from '../components/admin/RoutesAdmin';
import StationsAdmin from '../components/admin/StationsAdmin';
import SchedulesAdmin from '../components/admin/SchedulesAdmin';
import styles from './AdminDashboard.module.css';
import Profile from '../pages/Profile';


export default function AdminDashboard() {
  const [activeTab, setActiveTab] = useState('routes');

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
            className={activeTab === 'stations' ? styles.active : ''}
            onClick={() => setActiveTab('stations')}
          >
            Станції
          </button>

          <button
            className={activeTab === 'routes' ? styles.active : ''}
            onClick={() => setActiveTab('routes')}
          >
            Маршрути
          </button>

          <button
            className={activeTab === 'schedules' ? styles.active : ''}
            onClick={() => setActiveTab('schedules')}
          >
            Розклади
          </button>
        </nav>
        <div className={styles.content}>
          {activeTab === 'profile' && <Profile />}            
          {activeTab === 'routes' && <RoutesAdmin />}
          {activeTab === 'stations' && <StationsAdmin />}
          {activeTab === 'schedules' && <SchedulesAdmin />}
        </div>
      </div>
    </>
  );
}