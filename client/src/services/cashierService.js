export async function searchBookingsByClientOrBookingId(clientId, bookingId, token) {
  let url = 'http://localhost:5086/api/Bookings/';
  if (clientId) url += `user/${clientId}`;
  if (bookingId) url += `${bookingId}`;
  const res = await fetch(url, {
    headers: {
      'Authorization': `Bearer ${token}`,
    },
  });
  if (!res.ok) throw new Error('Не вдалося знайти бронювання');
  return await res.json();
}

export async function payBooking({ bookingId, cashierId, amount, paymentMethodId }, token) {
  const res = await fetch(`http://localhost:5086/api/Payments`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
    body: JSON.stringify({ bookingId, cashierId, amount, paymentMethodId }),
  });
  if (!res.ok) throw new Error('Не вдалося провести оплату');
  return await res.json();
}