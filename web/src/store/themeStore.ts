import { create } from 'zustand';

interface ThemeState {
  isDark: boolean;
  toggle: () => void;
}

export const useThemeStore = create<ThemeState>((set) => ({
  isDark: localStorage.getItem('theme') === 'dark',
  toggle: () => set(s => {
    const next = !s.isDark;
    localStorage.setItem('theme', next ? 'dark' : 'light');
    return { isDark: next };
  }),
}));
