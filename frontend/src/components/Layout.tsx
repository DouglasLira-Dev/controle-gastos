import React from 'react';
import { AppBar, Toolbar, Typography, Button, Container, Box } from '@mui/material';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

interface LayoutProps {
  children: React.ReactNode;
}

export const Layout: React.FC<LayoutProps> = ({ children }) => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            🏦 Controle de Gastos
          </Typography>
          {user && (
            <>
              <Button color="inherit" component={Link} to="/pessoas">
                Pessoas
              </Button>
              <Button color="inherit" component={Link} to="/transacoes">
                Transações
              </Button>
              <Button color="inherit" component={Link} to="/totais">
                Totais
              </Button>
              <Typography variant="body2" sx={{ mx: 2 }}>
                {user.username} ({user.role})
              </Typography>
              <Button color="inherit" onClick={handleLogout}>
                Sair
              </Button>
            </>
          )}
        </Toolbar>
      </AppBar>

      <Container maxWidth="lg" sx={{ mt: 4, mb: 4, flex: 1 }}>
        {children}
      </Container>

      <Box component="footer" sx={{ py: 3, bgcolor: 'grey.100', textAlign: 'center' }}>
        <Typography variant="body2" color="text.secondary">
          Desafio Controle de Gastos - Estágio TI © {new Date().getFullYear()}
        </Typography>
      </Box>
    </Box>
  );
};