# 🧠 Optical Text Recognition Trainer in ML.NET

![Screenshot](https://i.ibb.co/pQgHy39/App-ML.png)

A single-window WPF application to train custom OCR models using ML.NET. Supports multiple machine learning algorithms including:

- SDCA (maximum entropy and non-calibrated)
- Limited memory BFGS
- Naive Bayes
- LightGBM
- TensorFlow

The application generates optical data from system-installed fonts and supports training on rotated glyphs to boost recognition accuracy. Testing showed that **LightGBM outperforms even Google’s Tesseract** in domain-specific OCR accuracy.

![Accuracy Stats](https://i.ibb.co/rHScR48/Accuracy.png "Accuracy")

---

## 🚀 Features

- Choose fonts installed on your system
- Customize character rotation for training data
- Train with 6 different algorithms
- Live font cart and training status
- Quick testing with visual results

---

## 🧪 Quick Tutorial

- 📂 **Font Browser** (left): browse and preview fonts
- 🛒 **Cart Panel** (right): add fonts for training
- ⚙️ **Settings Panel** (center): configure rotation and algorithm
- 🔄 **F5**: Train the model with selected engine and font cart
- 🧪 **F9**: Test selected model against current font settings

---

## ⌨️ Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `ENTER` | Add current font to cart |
| `DELETE` | Remove current font from cart |
| `→` / `←` | Navigate next/previous font |
| `Shift + DELETE` | Clear the entire font cart |
| `F5` | Train model |
| `F8` | Test accuracy (not recommended) |
| `F9` | Test selected font model |

---

## 📄 License

MIT License © 2023 Mahendra Goyal

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

> The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

**THE SOFTWARE IS PROVIDED "AS IS"**, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
