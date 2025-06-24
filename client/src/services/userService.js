export async function getUserProfile(userId, token) {
  const res = await fetch(`http://localhost:5086/api/Users/${userId}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!res.ok) throw new Error('Не вдалося отримати дані профілю');

  return await res.json();
}

export async function updateUserProfile(userId, data, token) {
  const res = await fetch(`http://localhost:5086/api/Users/${userId}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });

  if (!res.ok) throw new Error('Не вдалося оновити профіль');
}
