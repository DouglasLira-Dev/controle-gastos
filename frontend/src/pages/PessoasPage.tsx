import React, { useState, useEffect } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  IconButton,
  Typography,
  Box,
  Chip,
  Alert,
  Snackbar,
  CircularProgress,
  Tooltip,
} from '@mui/material';
import { Delete, Add, Person, Visibility } from '@mui/icons-material';
import { pessoasApi } from '../api/api';
import type { Pessoa, PessoaCreate } from '../types';

const PessoasPage: React.FC = () => {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [openDialog, setOpenDialog] = useState(false);
  const [loading, setLoading] = useState(false);
  const [snackbar, setSnackbar] = useState({ 
    open: false, 
    message: '', 
    severity: 'success' as 'success' | 'error' 
  });
  const [formData, setFormData] = useState<PessoaCreate>({ nome: '', idade: 0 });
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    carregarPessoas();
  }, []);

  const carregarPessoas = async () => {
    try {
      setLoading(true);
      const data = await pessoasApi.getAll();
      setPessoas(data);
    } catch (error) {
      console.error('Erro ao carregar pessoas:', error);
      setSnackbar({ open: true, message: 'Erro ao carregar pessoas', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = () => {
    setFormData({ nome: '', idade: 0 });
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
  };

  const handleSubmit = async () => {
    try {
      if (!formData.nome.trim()) {
        setSnackbar({ open: true, message: 'Nome é obrigatório', severity: 'error' });
        return;
      }
      if (formData.idade < 0 || formData.idade > 150) {
        setSnackbar({ open: true, message: 'Idade deve estar entre 0 e 150', severity: 'error' });
        return;
      }

      await pessoasApi.create(formData);
      setSnackbar({ open: true, message: 'Pessoa criada com sucesso!', severity: 'success' });
      handleCloseDialog();
      carregarPessoas();
    } catch (error: any) {
      console.error('Erro ao criar pessoa:', error);
      const message = error.response?.data?.errors?.[0] || 'Erro ao criar pessoa';
      setSnackbar({ open: true, message, severity: 'error' });
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Tem certeza que deseja deletar esta pessoa? Todas as transações serão deletadas.')) {
      return;
    }

    try {
      setDeletingId(id);
      await pessoasApi.delete(id);
      setSnackbar({ open: true, message: 'Pessoa deletada com sucesso!', severity: 'success' });
      carregarPessoas();
    } catch (error) {
      console.error('Erro ao deletar pessoa:', error);
      setSnackbar({ open: true, message: 'Erro ao deletar pessoa', severity: 'error' });
    } finally {
      setDeletingId(null);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  };

  if (loading) {
    return (
      <Box sx= {{ display: "flex", justifyContent: "center", alignItems: "center", minHeight: "60vh" }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 3 }}>
        <Typography variant="h4" component="h1">
          👤 Pessoas
        </Typography>
        <Button
          variant="contained"
          color="primary"
          startIcon={<Add />}
          onClick={handleOpenDialog}
        >
          Nova Pessoa
        </Button>
      </Box>

      {pessoas.length === 0 ? (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <Person sx={{ fontSize: 60, color: 'grey.400', mb: 2 }} />
          <Typography variant="h6" color="text.secondary">
            Nenhuma pessoa cadastrada
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
            Clique em "Nova Pessoa" para começar
          </Typography>
        </Paper>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell><strong>Nome</strong></TableCell>
                <TableCell><strong>Idade</strong></TableCell>
                <TableCell><strong>Status</strong></TableCell>
                <TableCell align="right"><strong>Receitas</strong></TableCell>
                <TableCell align="right"><strong>Despesas</strong></TableCell>
                <TableCell align="right"><strong>Saldo</strong></TableCell>
                <TableCell align="center"><strong>Ações</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {pessoas.map((pessoa) => (
                <TableRow key={pessoa.id}>
                  <TableCell>{pessoa.nome}</TableCell>
                  <TableCell>{pessoa.idade} anos</TableCell>
                  <TableCell>
                    <Chip
                      label={pessoa.isMenorDeIdade ? 'Menor de Idade' : 'Adulto'}
                      color={pessoa.isMenorDeIdade ? 'warning' : 'success'}
                      size="small"
                    />
                  </TableCell>
                  <TableCell align="right" sx={{ color: 'success.main' }}>
                    {formatCurrency(pessoa.totalReceitas)}
                  </TableCell>
                  <TableCell align="right" sx={{ color: 'error.main' }}>
                    {formatCurrency(pessoa.totalDespesas)}
                  </TableCell>
                  <TableCell align="right" sx={{ 
                    fontWeight: 'bold',
                    color: pessoa.saldo >= 0 ? 'success.main' : 'error.main' 
                  }}>
                    {formatCurrency(pessoa.saldo)}
                  </TableCell>
                  <TableCell align="center">
                    <Tooltip title="Ver detalhes">
                      <IconButton size="small" color="info">
                        <Visibility />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Deletar">
                      <IconButton 
                        size="small" 
                        color="error"
                        onClick={() => handleDelete(pessoa.id)}
                        disabled={deletingId === pessoa.id}
                      >
                        {deletingId === pessoa.id ? <CircularProgress size={24} /> : <Delete />}
                      </IconButton>
                    </Tooltip>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {/* Dialog para criar pessoa */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>Nova Pessoa</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Nome"
            type="text"
            fullWidth
            required
            value={formData.nome}
            onChange={(e) => setFormData({ ...formData, nome: e.target.value })}
            sx={{ mt: 1 }}
          />
          <TextField
            margin="dense"
            label="Idade"
            type="number"
            fullWidth
            required
            value={formData.idade || ''}
            onChange={(e) => setFormData({ ...formData, idade: parseInt(e.target.value) || 0 })}
            slotProps={{ htmlInput: { min: 0, max: 150 } }}
            sx={{ mt: 2 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancelar</Button>
          <Button onClick={handleSubmit} variant="contained" color="primary">
            Criar
          </Button>
        </DialogActions>
      </Dialog>

      {/* Snackbar para notificações */}
      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert severity={snackbar.severity} onClose={() => setSnackbar({ ...snackbar, open: false })}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
};

export default PessoasPage;