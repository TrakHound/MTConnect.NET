/**
 * VitePress theme entry for the MTConnect.NET documentation site.
 *
 * Extends the default theme and layers a small CSS override that
 * remaps the brand palette to the TrakHound accent (`#0073e6`).
 * Keeping the override in `./style.css` (rather than inlining a
 * `<style>` block) lets VitePress hash the asset for cache-busting
 * on each build.
 *
 * @see {@link https://vitepress.dev/guide/extending-default-theme}
 */
import DefaultTheme from 'vitepress/theme';
import './style.css';

export default DefaultTheme;
