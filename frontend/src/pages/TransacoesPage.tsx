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
  Typography,
  Box,
  Chip,
  Alert,
  Snackbar,
  CircularProgress,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Tooltip,
} from '@mui/material';
import { Add, TrendingUp, TrendingDown } from '@mui/icons-material';
import { transacoesApi, pessoasApi } from '../api/api';
import { type Transacao, type TransacaoCreate, type Pessoa, TipoTransacao } from '../types';

const TransacoesPage: React.FC = () => {
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [openDialog, setOpenDialog] = useState(false);
  const [loading, setLoading] = useState(false);
  const [snackbar, setSnackbar] = useState({ 
    open: false, 
    message: '', 
    severity: 'success' as 'success' | 'error' 
  });
  const [formData, setFormData] = useState<TransacaoCreate>({
    descricao: '',
    valor: 0,
    tipo: TipoTransacao.Despesa,
    pessoaId: '',
  });

  useEffect(() => {
    carregarDados();
  }, []);

  const carregarDados = async () => {
    try {
      setLoading(true);
      const [transacoesData, pessoasData] = await Promise.all([
        transacoesApi.getAll(),
        pessoasApi.getAll(),
      ]);
      setTransacoes(transacoesData);
      setPessoas(pessoasData);
    } catch (error) {
      console.error('Erro ao carregar dados:', error);
      setSnackbar({ open: true, message: 'Erro ao carregar dados', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = () => {
    setFormData({ descricao: '', valor: 0, tipo: TipoTransacao.Despesa, pessoaId: '' });
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
  };

  const handleSubmit = async () => {
  try {
    // 🔧 VALIDAÇÕES
    if (!formData.descricao.trim()) {
      setSnackbar({ open: true, message: 'Descrição é obrigatória', severity: 'error' });
      return;
    }
    if (formData.valor <= 0) {
      setSnackbar({ open: true, message: 'Valor deve ser maior que zero', severity: 'error' });
      return;
    }
    if (!formData.pessoaId) {
      setSnackbar({ open: true, message: 'Selecione uma pessoa', severity: 'error' });
      return;
    }

    // 🔧 LOG DOS DADOS ENVIADOS
    console.log('📤 Enviando transação:', {
      descricao: formData.descricao,
      valor: formData.valor,
      tipo: formData.tipo,
      pessoaId: formData.pessoaId
    });

    // 🔧 GARANTIR QUE OS TIPOS ESTÃO CORRETOS
    const dataToSend = {
      descricao: formData.descricao,
      valor: Number(formData.valor),
      tipo: Number(formData.tipo),
      pessoaId: formData.pessoaId
    };

    await transacoesApi.create(dataToSend);
    setSnackbar({ open: true, message: 'Transação criada com sucesso!', severity: 'success' });
    handleCloseDialog();
    carregarDados();
  } catch (error: any) {
    console.error('❌ Erro ao criar transação:', error);
    console.error('📄 Resposta do servidor:', error.response?.data);
    
    // 🔧 MENSAGEM DE ERRO MAIS DETALHADA
    let message = 'Erro ao criar transação';
    if (error.response?.data?.errors) {
      // Erros do FluentValidation
      const errors = error.response.data.errors;
      if (Array.isArray(errors)) {
        message = errors.join(', ');
      } else if (typeof errors === 'object') {
        message = Object.values(errors).flat().join(', ');
      }
    } else if (error.response?.data?.error) {
      message = error.response.data.error;
    }
    
    setSnackbar({ open: true, message, severity: 'error' });
  }
};

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const getTipoColor = (tipo: TipoTransacao) => {
    return tipo === TipoTransacao.Receita ? 'success' : 'error';
  };

  const getTipoIcon = (tipo: TipoTransacao) => {
    return tipo === TipoTransacao.Receita ? <TrendingUp /> : <TrendingDown />;
  };

  if (loading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", minHeight: "60vh" }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 3 }}>
        <Typography variant="h4" component="h1">
          💰 Transações
        </Typography>
        <Button
          variant="contained"
          color="primary"
          startIcon={<Add />}
          onClick={handleOpenDialog}
        >
          Nova Transação
        </Button>
      </Box>

      {transacoes.length === 0 ? (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <Typography variant="h6" color="text.secondary">
            Nenhuma transação cadastrada
          </Typography>
        </Paper>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell><strong>Descrição</strong></TableCell>
                <TableCell><strong>Pessoa</strong></TableCell>
                <TableCell><strong>Data</strong></TableCell>
                <TableCell><strong>Tipo</strong></TableCell>
                <TableCell align="right"><strong>Valor</strong></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {transacoes.map((transacao) => (
                <TableRow key={transacao.id}>
                  <TableCell>{transacao.descricao}</TableCell>
                  <TableCell>{transacao.nomePessoa}</TableCell>
                  <TableCell>{formatDate(transacao.dataCriacao)}</TableCell>
                  <TableCell>
                    <Chip
                      icon={getTipoIcon(transacao.tipo)}
                      label={transacao.tipoDescricao}
                      color={getTipoColor(transacao.tipo)}
                      size="small"
                    />
                  </TableCell>
                  <TableCell align="right" sx={{ 
                    fontWeight: 'bold',
                    color: transacao.tipo === TipoTransacao.Receita ? 'success.main' : 'error.main' 
                  }}>
                    {formatCurrency(transacao.valor)}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {/* Dialog para criar transação */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>Nova Transação</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Descrição"
            type="text"
            fullWidth
            required
            value={formData.descricao}
            onChange={(e) => setFormData({ ...formData, descricao: e.target.value })}
            sx={{ mt: 1 }}
          />
          <TextField
            margin="dense"
            label="Valor"
            type="number"
            fullWidth
            required
            value={formData.valor || ''}
            onChange={(e) => setFormData({ ...formData, valor: parseFloat(e.target.value) || 0 })}
            slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
            sx={{ mt: 2 }}
          />
          <FormControl fullWidth margin="dense" sx={{ mt: 2 }}>
            <InputLabel>Tipo</InputLabel>
            <Select
              value={formData.tipo}
              label="Tipo"
              onChange={(e) => setFormData({ ...formData, tipo: Number(e.target.value) as TipoTransacao })}
            >
              <MenuItem value={TipoTransacao.Receita}>Receita</MenuItem>
              <MenuItem value={TipoTransacao.Despesa}>Despesa</MenuItem>
            </Select>
          </FormControl>
          <FormControl fullWidth margin="dense" sx={{ mt: 2 }}>
            <InputLabel>Pessoa</InputLabel>
            <Select
              value={formData.pessoaId}
              label="Pessoa"
              onChange={(e) => setFormData({ ...formData, pessoaId: e.target.value })}
            >
              {pessoas.map((pessoa) => (
                <MenuItem key={pessoa.id} value={pessoa.id}>
                  {pessoa.nome} ({pessoa.idade} anos) {pessoa.isMenorDeIdade ? '⚠️ Menor' : ''}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          {formData.pessoaId && (
            <Box sx={{ mt: 2, p: 1, bgcolor: 'grey.50', borderRadius: 1 }}>
              <Typography variant="caption" color="text.secondary">
                💡 Dica: Pessoas menores de 18 anos só podem cadastrar despesas
              </Typography>
            </Box>
          )}
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

export default TransacoesPage;