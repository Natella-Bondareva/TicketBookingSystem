import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import PrivateRoute from './routes/PrivateRoute';
import UserProfile from './pages/UserProfile';
import CashierPage from './pages/CashierPage';
import AdminDashboard from './pages/AdminDashboard';



function App() {

  return (
    
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        <Route path="/cashier" element={
          <PrivateRoute roles={['cashier']}>
            <CashierPage />
          </PrivateRoute>
        } />

        <Route path="/profile" element={
          <PrivateRoute roles={['client']}>
            <UserProfile />
          </PrivateRoute>
        } />

        <Route path="/admin" element={
          <PrivateRoute roles={['admin']}>
            <AdminDashboard />
          </PrivateRoute>
        } />

        <Route path="/profile" element={
          <PrivateRoute roles={['admin', 'cashier']}>
            {/*<ProfilePage />*/}
          </PrivateRoute>
        } />
      </Routes>
  );
}

export default App;

