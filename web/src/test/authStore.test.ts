import { beforeEach, describe, expect, it } from 'vitest';
import { useAuthStore } from '../store/authStore';

// Reset store state between tests
beforeEach(() => {
  useAuthStore.setState({ accessToken: null, role: null, userId: null });
});

describe('authStore', () => {
  describe('isAuthenticated', () => {
    it('returns false when no token is set', () => {
      expect(useAuthStore.getState().isAuthenticated()).toBe(false);
    });

    it('returns true after setAuth is called', () => {
      useAuthStore.getState().setAuth('tok', 'ManagementCompany', 'user-1');
      expect(useAuthStore.getState().isAuthenticated()).toBe(true);
    });

    it('returns false after logout', () => {
      useAuthStore.getState().setAuth('tok', 'ManagementCompany', 'user-1');
      useAuthStore.getState().logout();
      expect(useAuthStore.getState().isAuthenticated()).toBe(false);
    });
  });

  describe('setAuth', () => {
    it('stores token, role and userId', () => {
      useAuthStore.getState().setAuth('my-token', 'Executor', 'u-42');
      const state = useAuthStore.getState();
      expect(state.accessToken).toBe('my-token');
      expect(state.role).toBe('Executor');
      expect(state.userId).toBe('u-42');
    });
  });

  describe('setToken', () => {
    it('updates only the access token, leaving role and userId intact', () => {
      useAuthStore.getState().setAuth('old-token', 'Customer', 'u-1');
      useAuthStore.getState().setToken('new-token');
      const state = useAuthStore.getState();
      expect(state.accessToken).toBe('new-token');
      expect(state.role).toBe('Customer');
      expect(state.userId).toBe('u-1');
    });
  });

  describe('logout', () => {
    it('clears all auth state', () => {
      useAuthStore.getState().setAuth('tok', 'ManagementCompany', 'admin');
      useAuthStore.getState().logout();
      const state = useAuthStore.getState();
      expect(state.accessToken).toBeNull();
      expect(state.role).toBeNull();
      expect(state.userId).toBeNull();
    });
  });
});
