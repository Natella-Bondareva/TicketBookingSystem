export async function bookRoute(data, token) {
  console.log('📤 Sending body:', JSON.stringify(data, null, 2));

  const res = await fetch('http://localhost:5086/api/Bookings/auto', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });

  if (!res.ok) {
    const error = await res.text();
    throw new Error(error || 'Не вдалося забронювати квиток');
  }

  return await res.json();
}

export async function getUserBookings(userId, token) {
  const res = await fetch(`http://localhost:5086/api/Bookings/user/${userId}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  if (!res.ok) throw new Error('Не вдалося отримати бронювання');
  return await res.json();
}

export async function cancelBooking(id, token) {
  const res = await fetch(`http://localhost:5086/api/Bookings/${id}/cancel`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  if (!res.ok) throw new Error('Не вдалося скасувати бронювання');
  return await res.json();
}
