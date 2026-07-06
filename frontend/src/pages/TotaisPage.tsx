import React, { useState, useEffect } from 'react';
import {
  Paper,
  Typography,
  Box,
  Grid,
  Card,
  CardContent,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  CircularProgress,
} from '@mui/material';
import { TrendingUp, TrendingDown, AccountBalance } from '@mui/icons-material';
import { totaisApi } from '../api/api';
import type { TotalGeral } from '../types';

const TotaisPage: React.FC = () => {
  const [totais, setTotais] = useState<TotalGeral | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    carregarTotais();
  }, []);

  const carregarTotais = async () => {
    try {
      setLoading(true);
      const data = await totaisApi.getTotais();
      setTotais(data);
    } catch (error) {
      console.error('Erro ao carregar totais:', error);
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
        <CircularProgress />
      </Box>
    );
  }

  if (!totais || totais.totaisPorPessoa.length === 0) {
    return (
      <Box>
        <Typography variant="h4" component="h1" gutterBottom>
          📊 Totais
        </Typography>
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <Typography variant="h6" color="text.secondary">
            Nenhum dado disponível
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
            Cadastre pessoas e transações para ver os totais
          </Typography>
        </Paper>
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" component="h1" gutterBottom>
        📊 Totais
      </Typography>

      {/* Cards de Totais Gerais */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} md={4}>
          <Card sx={{ bgcolor: 'success.light', color: 'white' }}>
            <CardContent>
              <Box display="flex" alignItems="center">
                <TrendingUp sx={{ fontSize: 40, mr: 2 }} />
                <Box>
                  <Typography variant="h6">Total Receitas</Typography>
                  <Typography variant="h4">{formatCurrency(totais.totalReceitasGeral)}</Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={4}>
          <Card sx={{ bgcolor: 'error.light', color: 'white' }}>
            <CardContent>
              <Box display="flex" alignItems="center">
                <TrendingDown sx={{ fontSize: 40, mr: 2 }} />
                <Box>
                  <Typography variant="h6">Total Despesas</Typography>
                  <Typography variant="h4">{formatCurrency(totais.totalDespesasGeral)}</Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={4}>
          <Card sx={{ 
            bgcolor: totais.saldoLiquido >= 0 ? 'info.light' : 'warning.light', 
            color: 'white' 
          }}>
            <CardContent>
              <Box display="flex" alignItems="center">
                <AccountBalance sx={{ fontSize: 40, mr: 2 }} />
                <Box>
                  <Typography variant="h6">Saldo Líquido</Typography>
                  <Typography variant="h4">{formatCurrency(totais.saldoLiquido)}</Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <Typography variant="h5" gutterBottom>
        Totais por Pessoa
      </Typography>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell><strong>Pessoa</strong></TableCell>
              <TableCell><strong>Idade</strong></TableCell>
              <TableCell align="right"><strong>Receitas</strong></TableCell>
              <TableCell align="right"><strong>Despesas</strong></TableCell>
              <TableCell align="right"><strong>Saldo</strong></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {totais.totaisPorPessoa.map((item) => (
              <TableRow key={item.pessoaId}>
                <TableCell>{item.nomePessoa}</TableCell>
                <TableCell>{item.idade} anos</TableCell>
                <TableCell align="right" sx={{ color: 'success.main' }}>
                  {formatCurrency(item.totalReceitas)}
                </TableCell>
                <TableCell align="right" sx={{ color: 'error.main' }}>
                  {formatCurrency(item.totalDespesas)}
                </TableCell>
                <TableCell align="right" sx={{ 
                  fontWeight: 'bold',
                  color: item.saldo >= 0 ? 'success.main' : 'error.main' 
                }}>
                  {formatCurrency(item.saldo)}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
};

export default TotaisPage;