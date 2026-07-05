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

const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await login({ username, password });
      navigate('/');
    } catch (err: any) {
      setError(err.response?.data?.error || 'Erro ao fazer login');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" component="h1" align="center" gutterBottom>
          🏦 Controle de Gastos
        </Typography>
        <Typography variant="body2" align="center" color="textSecondary" sx={{ mb: 3 }}>
          Faça login para acessar o sistema
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <form onSubmit={handleSubmit}>
          <TextField
            fullWidth
            label="Usuário ou Email"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            margin="normal"
            required
            disabled={loading}
          />
          <TextField
            fullWidth
            label="Senha"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
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
            {loading ? <CircularProgress size={24} /> : 'Entrar'}
          </Button>
        </form>

        <Box sx={{ mt: 2, textAlign: 'center' }}>
          <Typography variant="body2">
            Não tem conta? <Link to="/register">Cadastre-se</Link>
          </Typography>
        </Box>
      </Paper>
    </Container>
  );
};

export default LoginPage;