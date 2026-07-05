import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { 
  Container, 
  Paper, 
  Typography, 
  TextField, 
  Button, 
  Box, 
  Alert,
  CircularProgress 
} from '@mui/material';
import { useAuth } from '../context/AuthContext';

const RegisterPage: React.FC = () => {
  const navigate = useNavigate();
  const { register } = useAuth();
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await register(formData);
      navigate('/');
    } catch (err: any) {
      setError(err.response?.data?.error || 'Erro ao criar conta');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" component="h1" align="center" gutterBottom>
          Criar Conta
        </Typography>
        <Typography variant="body2" align="center" color="textSecondary" sx={{ mb: 3 }}>
          Registre-se para começar a usar o sistema
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <form onSubmit={handleSubmit}>
          <TextField
            fullWidth
            label="Usuário"
            name="username"
            value={formData.username}
            onChange={handleChange}
            margin="normal"
            required
            disabled={loading}
          />
          <TextField
            fullWidth
            label="Email"
            name="email"
            type="email"
            value={formData.email}
            onChange={handleChange}
            margin="normal"
            required
            disabled={loading}
          />
          <TextField
            fullWidth
            label="Senha"
            name="password"
            type="password"
            value={formData.password}
            onChange={handleChange}
            margin="normal"
            required
            disabled={loading}
          />
          <TextField
            fullWidth
            label="Confirmar Senha"
            name="confirmPassword"
            type="password"
            value={formData.confirmPassword}
            onChange={handleChange}
            margin="normal"
            required
            disabled={loading}
          />
          <Button
            fullWidth
            type="submit"
            variant="contained"
            color="primary"
            size="large"
            disabled={loading}
            sx={{ mt: 3 }}
          >
            {loading ? <CircularProgress size={24} /> : 'Criar Conta'}
          </Button>
        </form>

        <Box sx={{ mt: 2, textAlign: 'center' }}>
          <Typography variant="body2">
            Já tem conta? <Link to="/login">Faça login</Link>
          </Typography>
        </Box>
      </Paper>
    </Container>
  );
};

export default RegisterPage;